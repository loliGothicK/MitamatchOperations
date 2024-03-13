using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Api.Gax;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Storage.v1.Data;
using Google.Cloud.Datastore.V1;
using Google.Cloud.Storage.V1;
using LiteDB;
using Mitama.Domain;
using Mitama.Pages.Common;
using Newtonsoft.Json;
using Object = Google.Apis.Storage.v1.Data.Object;

namespace Mitama.Repository;

internal class Repository
{
    private static readonly GoogleCredential credential = GoogleCredential.FromJson(JsonConvert.SerializeObject(new
    {
        type = "service_account",
        project_id = "assaultlily",
        private_key_id = "13b3e809d5e493489d67018ac1d69d5c2e2eaa04",
        private_key = "GOOGLE_CLOUD_PRIVATE_KEY",
        client_email = "mitamatch@assaultlily.iam.gserviceaccount.com",
        client_id = "116107053801726389433",
        auth_uri = "https://accounts.google.com/o/oauth2/auth",
        token_uri = "https://oauth2.googleapis.com/token",
        auth_provider_x509_cert_url = "https://www.googleapis.com/oauth2/v1/certs",
        client_x509_cert_url = "https://www.googleapis.com/robot/v1/metadata/x509/mitamatch%40assaultlily.iam.gserviceaccount.com",
        universe_domain = "googleapis.com"
    }));

    public class Storage
    {
        private static readonly string buketName = "mitamatch";

        private static readonly Lazy<StorageClient> Client = new(() =>
        {
            return StorageClient.Create(credential);
        });

        public static PagedEnumerable<Objects, Object> ListObjects(string prefix)
        {
            return Client.Value.ListObjects(buketName, prefix);
        }

        public static Object DownloadObject(string prefix, Stream stream)
        {
            return Client.Value.DownloadObject(buketName, prefix, stream);
        }
    }

    public class DataStore
    {
        private static readonly Lazy<DatastoreDb> DB = new(() =>
        {
            var client = new DatastoreClientBuilder() { Credential = credential }.Build();
            return DatastoreDb.Create("assaultlily", string.Empty, client);
        });

        public static void Upsert(DiscordUser user)
        {
            var key = DB.Value.CreateKeyFactory("user").CreateKey(user.id);
            var result = DB.Value.Lookup(key);
            var entity = new Entity()
            {
                Key = key,
                ["id"] = user.id,
                ["avatar"] = user.avatar,
                ["username"] = user.username,
                ["discriminator"] = user.discriminator,
                ["global_name"] = user.global_name,
                ["email"] = user.email,
                ["version"] = Pages.Common.Version.Current.ToString(),
                ["updated_at"] = DateTime.UtcNow
            };

            if (result is not null)
            {
                entity["created_at"] = result["created_at"];
                DB.Value.Upsert(entity);
            }
            else
            {
                entity["created_at"] = DateTime.UtcNow;
                DB.Value.Upsert(entity);
            }
        }
    }

    public class LiteDB
    {
        public static IEnumerable<T> List<T>() where T : IModel
        {
            using var database = new LiteDatabase($@"{Director.DatabaseDir()}\data");
            foreach (var item in database.GetCollection<T>(T.Name()).FindAll())
            {
                yield return item;
            }
        }

        public static Task ExtractImages()
        {
            var cache = Director.ReadCache();

            var index = cache.MemoriaIndex ?? -1;
            foreach (var chunk in Domain.Memoria.List.Value.Where(m => m.Id > index).Chunk(40))
            {
                using var database = new LiteDatabase($@"{Director.DatabaseDir()}\data");
                foreach (var memoria in chunk)
                {
                    database.FileStorage.FindById($"$/memoria/{memoria.Name}.png").SaveAs($@"{Director.MemoriaImageDir()}\{memoria.Name}.png");
                }
            }

            index = cache.CostumeIndex ?? -1;
            foreach (var chunk in Domain.Costume.List.Value.Where(c => c.Index > index).Chunk(40))
            {
                using var database = new LiteDatabase($@"{Director.DatabaseDir()}\data");
                foreach (var costume in chunk)
                {
                    database.FileStorage.FindById($"$/costume/{costume.Lily}/{costume.Name}.png").SaveAs($@"{Director.CostumeImageDir(costume.Lily)}\{costume.Name}.png");
                }
            }

            index = cache.OrderIndex ?? -1;
            foreach (var chunk in Domain.Order.List.Value.Where(o => o.Index > index).Chunk(40))
            {
                using var database = new LiteDatabase($@"{Director.DatabaseDir()}\data");
                foreach (var order in chunk)
                {
                    database.FileStorage.FindById($"$/order/{order.Name}.png").SaveAs($@"{Director.OrderImageDir()}\{order.Name}.png");
                }
            }

            index = cache.CharmIndex ?? -1;
            foreach (var chunk in Domain.Charm.List.Value.Where(c => c.Index > index).Chunk(40))
            {
                using var database = new LiteDatabase($@"{Director.DatabaseDir()}\data");
                foreach (var charm in chunk)
                {
                    database.FileStorage.FindById($"$/charm/{charm.Name}.png").SaveAs($@"{Director.CharmImageDir()}\{charm.Name}.png");
                }
            }

            Director.CacheWrite((cache with
            {
                MemoriaIndex = MaxId<Memoria.POCO>("memoria"),
                CostumeIndex = MaxId<Costume.POCO>("costume"),
                OrderIndex = MaxId<Order.POCO>("order"),
                CharmIndex = MaxId<Charm.POCO>("charm"),
                Version = Pages.Common.Version.Current,
            }).ToJsonBytes());

            return Task.CompletedTask;
        }

        private static int MaxId<T>(string table)
        {
            try
            {
                using var database = new LiteDatabase($@"{Director.DatabaseDir()}\data");
                var col = database.GetCollection<T>(table);
                var max = col.Max("_id");
                return max.RawValue is null ? -1 : (int)max.RawValue;
            }
            catch
            {
                return -1;
            }
        }
    }
}
