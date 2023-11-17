using System;

namespace mitama.Domain;

public record struct Costume(string Lily = "", string Name = "", string Type = "") {
    public Uri Uri => new($"ms-appx:///Assets/costume/{Lily}/{Name}.jpg");
    public string Path = $"/Assets/costume/{Lily}/{Name}.jpg";

    public bool IsFront => Type switch {
        "通常単体" or "通常範囲" or "特殊単体" or "特殊範囲" => true,
        _ => false,
    };
    public bool IsBack => Type switch {
        "支援" or "妨害" or "回復" => true,
        _ => false,
    };

    public bool CanBeEquipped(Memoria memoria) => memoria.Kind switch {
        Vanguard => IsFront,
        Rearguard => IsBack,
        _ => throw new ArgumentOutOfRangeException()
    };

    public static Costume[] List => new[]
    {
        new Costume
        {
            Lily = "一柳梨璃",
            Name = "百合ヶ丘標準制服",
            Type = "通常単体"
        },
        new Costume
        {
            Lily = "一柳梨璃",
            Name = "アラウンドザウィロー",
            Type = "支援"
        },
        new Costume
        {
            Lily = "一柳梨璃",
            Name = "ムーンライト",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "一柳梨璃",
            Name = "ナイトルージュ",
            Type = "回復"
        },
        new Costume
        {
            Lily = "一柳梨璃",
            Name = "百合ヶ丘訓練制服",
            Type = "特殊単体"
        },
        new Costume
        {
            Lily = "一柳梨璃",
            Name = "GROWING",
            Type = "支援"
        },
        new Costume
        {
            Lily = "一柳梨璃",
            Name = "リリティカルサマー",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "一柳梨璃",
            Name = "ラプラスの目覚め",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "一柳梨璃",
            Name = "ライトウィンド",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "一柳梨璃",
            Name = "ブリリアントスピカ",
            Type = "支援"
        },
        new Costume
        {
            Lily = "一柳梨璃",
            Name = "ファンシーフラワー",
            Type = "回復"
        },
        new Costume
        {
            Lily = "一柳梨璃",
            Name = "聖夜のプレゼント",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "一柳梨璃",
            Name = "リディアン音楽院制服",
            Type = "回復"
        },
        new Costume
        {
            Lily = "一柳梨璃",
            Name = "ハピネスブーケ",
            Type = "特殊単体"
        },
        new Costume
        {
            Lily = "一柳梨璃",
            Name = "メイドバレンタイン",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "一柳梨璃",
            Name = "ブリッツアングリフ",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "一柳梨璃",
            Name = "秦祀隊試作レギオン制服",
            Type = "支援"
        },
        new Costume
        {
            Lily = "一柳梨璃",
            Name = "グロリアスカラー",
            Type = "支援"
        },
        new Costume
        {
            Lily = "一柳梨璃",
            Name = "りりふぇす!!フラワー",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "一柳梨璃",
            Name = "星花の浴衣",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "白井夢結",
            Name = "百合ヶ丘標準制服",
            Type = "特殊単体"
        },
        new Costume
        {
            Lily = "白井夢結",
            Name = "アラウンドザウィロー",
            Type = "回復"
        },
        new Costume
        {
            Lily = "白井夢結",
            Name = "ルナティックトランサー",
            Type = "通常単体"
        },
        new Costume
        {
            Lily = "白井夢結",
            Name = "ムーンライト",
            Type = "支援"
        },
        new Costume
        {
            Lily = "白井夢結",
            Name = "ナイトルージュ",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "白井夢結",
            Name = "百合ヶ丘訓練制服",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "白井夢結",
            Name = "リリティカルサマー",
            Type = "通常単体"
        },
        new Costume
        {
            Lily = "白井夢結",
            Name = "エレガントストライカー",
            Type = "通常単体"
        },
        new Costume
        {
            Lily = "白井夢結",
            Name = "ブリリアントスピカ",
            Type = "特殊単体"
        },
        new Costume
        {
            Lily = "白井夢結",
            Name = "GROWING",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "白井夢結",
            Name = "聖夜のプレゼント",
            Type = "特殊単体"
        },
        new Costume
        {
            Lily = "白井夢結",
            Name = "リディアン音楽院制服",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "白井夢結",
            Name = "幸福晴着",
            Type = "特殊単体"
        },
        new Costume
        {
            Lily = "白井夢結",
            Name = "ハピネスブーケ",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "白井夢結",
            Name = "メイドホワイトデー",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "白井夢結",
            Name = "秦祀隊試作レギオン制服",
            Type = "回復"
        },
        new Costume
        {
            Lily = "白井夢結",
            Name = "りりふぇす!!フラワー",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "白井夢結",
            Name = "ブリッツアングリフ",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "楓・J・ヌーベル",
            Name = "百合ヶ丘標準制服",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "楓・J・ヌーベル",
            Name = "アラウンドザウィロー",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "楓・J・ヌーベル",
            Name = "ブリリアントスピカ",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "楓・J・ヌーベル",
            Name = "アーセナリーローズ",
            Type = "回復"
        },
        new Costume
        {
            Lily = "楓・J・ヌーベル",
            Name = "百合ヶ丘訓練制服",
            Type = "支援"
        },
        new Costume
        {
            Lily = "楓・J・ヌーベル",
            Name = "リリティカルサマー",
            Type = "回復"
        },
        new Costume
        {
            Lily = "楓・J・ヌーベル",
            Name = "聖夜のプレゼント",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "楓・J・ヌーベル",
            Name = "水夕会試作隊服",
            Type = "支援"
        },
        new Costume
        {
            Lily = "楓・J・ヌーベル",
            Name = "星花の浴衣",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "二川二水",
            Name = "百合ヶ丘標準制服",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "二川二水",
            Name = "アラウンドザウィロー",
            Type = "支援"
        },
        new Costume
        {
            Lily = "二川二水",
            Name = "ブリリアントスピカ",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "二川二水",
            Name = "アーセナリーローズ",
            Type = "特殊単体"
        },
        new Costume
        {
            Lily = "二川二水",
            Name = "百合ヶ丘訓練制服",
            Type = "回復"
        },
        new Costume
        {
            Lily = "二川二水",
            Name = "涼風の浴衣",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "二川二水",
            Name = "ヘイムスクリングラ制服",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "二川二水",
            Name = "水夕会試作隊服",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "二川二水",
            Name = "リリティカルサマー",
            Type = "回復"
        },
        new Costume
        {
            Lily = "安藤鶴紗",
            Name = "百合ヶ丘標準制服",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "安藤鶴紗",
            Name = "アラウンドザウィロー",
            Type = "回復"
        },
        new Costume
        {
            Lily = "安藤鶴紗",
            Name = "ナイトルージュ",
            Type = "通常単体"
        },
        new Costume
        {
            Lily = "安藤鶴紗",
            Name = "百合ヶ丘訓練制服",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "安藤鶴紗",
            Name = "防衛軍式典制服",
            Type = "特殊単体"
        },
        new Costume
        {
            Lily = "安藤鶴紗",
            Name = "サマースタイル",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "安藤鶴紗",
            Name = "ふしぎの国のハロウィン",
            Type = "回復"
        },
        new Costume
        {
            Lily = "安藤鶴紗",
            Name = "ブリリアントスピカ",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "安藤鶴紗",
            Name = "グロリアスカラー",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "安藤鶴紗",
            Name = "リリティカルサマー",
            Type = "支援"
        },
        new Costume
        {
            Lily = "安藤鶴紗",
            Name = "ブリッツアングリフ",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "吉村・Thi・梅",
            Name = "百合ヶ丘標準制服",
            Type = "通常単体"
        },
        new Costume
        {
            Lily = "吉村・Thi・梅",
            Name = "アラウンドザウィロー",
            Type = "支援"
        },
        new Costume
        {
            Lily = "吉村・Thi・梅",
            Name = "ムーンライト",
            Type = "特殊単体"
        },
        new Costume
        {
            Lily = "吉村・Thi・梅",
            Name = "百合ヶ丘訓練制服",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "吉村・Thi・梅",
            Name = "エレガントストライカー",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "吉村・Thi・梅",
            Name = "ふしぎの国のハロウィン",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "吉村・Thi・梅",
            Name = "防衛軍式典制服",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "吉村・Thi・梅",
            Name = "リリティカルサマー",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "吉村・Thi・梅",
            Name = "ブリッツアングリフ",
            Type = "支援"
        },
        new Costume
        {
            Lily = "郭神琳",
            Name = "百合ヶ丘標準制服",
            Type = "特殊単体"
        },
        new Costume
        {
            Lily = "郭神琳",
            Name = "アラウンドザウィロー",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "郭神琳",
            Name = "百合ヶ丘訓練制服",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "郭神琳",
            Name = "防衛軍式典制服",
            Type = "支援"
        },
        new Costume
        {
            Lily = "郭神琳",
            Name = "ブリリアントスピカ",
            Type = "通常単体"
        },
        new Costume
        {
            Lily = "郭神琳",
            Name = "リリティカルサマー",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "郭神琳",
            Name = "ヘイムスクリングラ制服",
            Type = "回復"
        },
        new Costume
        {
            Lily = "郭神琳",
            Name = "ファストブレイカー",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "郭神琳",
            Name = "幸福晴着",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "王雨嘉",
            Name = "百合ヶ丘標準制服",
            Type = "通常単体"
        },
        new Costume
        {
            Lily = "王雨嘉",
            Name = "アラウンドザウィロー",
            Type = "回復"
        },
        new Costume
        {
            Lily = "王雨嘉",
            Name = "百合ヶ丘訓練制服",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "王雨嘉",
            Name = "防衛軍式典制服",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "王雨嘉",
            Name = "ブリリアントスピカ",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "王雨嘉",
            Name = "リリティカルサマー",
            Type = "通常単体"
        },
        new Costume
        {
            Lily = "王雨嘉",
            Name = "ヘイムスクリングラ制服",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "王雨嘉",
            Name = "幸福晴着",
            Type = "支援"
        },
        new Costume
        {
            Lily = "王雨嘉",
            Name = "和装猫耳メイド",
            Type = "回復"
        },
        new Costume
        {
            Lily = "王雨嘉",
            Name = "ブリッツアングリフ",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "王雨嘉",
            Name = "グロリアスカラー",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "ミリアム",
            Name = "百合ヶ丘標準制服",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "ミリアム",
            Name = "アラウンドザウィロー",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "ミリアム",
            Name = "ブリリアントスピカ",
            Type = "通常単体"
        },
        new Costume
        {
            Lily = "ミリアム",
            Name = "アーセナリーローズ",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "ミリアム",
            Name = "百合ヶ丘訓練制服",
            Type = "回復"
        },
        new Costume
        {
            Lily = "ミリアム",
            Name = "レゾナンスオブハート",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "ミリアム",
            Name = "ふしぎの国のハロウィン",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "ミリアム",
            Name = "ムーンライト",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "ミリアム",
            Name = "メイドバレンタイン",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "ミリアム",
            Name = "リリティカルサマー",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "ミリアム",
            Name = "星花の浴衣",
            Type = "回復"
        },
        new Costume
        {
            Lily = "ミリアム",
            Name = "ブリッツアングリフ",
            Type = "支援"
        },
        new Costume
        {
            Lily = "真島百由",
            Name = "百合ヶ丘標準制服",
            Type = "支援"
        },
        new Costume
        {
            Lily = "真島百由",
            Name = "レゾナンスオブハート",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "真島百由",
            Name = "アーセナリーローズ",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "真島百由",
            Name = "ムーンライト",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "真島百由",
            Name = "メイドホワイトデー",
            Type = "通常単体"
        },
        new Costume
        {
            Lily = "一柳結梨",
            Name = "百合ヶ丘標準制服",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "一柳結梨",
            Name = "GROWING",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "一柳結梨",
            Name = "ファンシーフラワー",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "天野天葉",
            Name = "百合ヶ丘標準制服",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "天野天葉",
            Name = "高難度外征レギオン制服",
            Type = "回復"
        },
        new Costume
        {
            Lily = "番匠谷依奈",
            Name = "百合ヶ丘標準制服",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "遠藤亜羅椰",
            Name = "百合ヶ丘標準制服",
            Type = "支援"
        },
        new Costume
        {
            Lily = "遠藤亜羅椰",
            Name = "高難度外征レギオン制服",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "田中壱",
            Name = "百合ヶ丘標準制服",
            Type = "通常単体"
        },
        new Costume
        {
            Lily = "田中壱",
            Name = "高難度外征レギオン制服",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "江川樟美",
            Name = "百合ヶ丘標準制服",
            Type = "回復"
        },
        new Costume
        {
            Lily = "金箱弥宙",
            Name = "百合ヶ丘標準制服",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "金箱弥宙",
            Name = "高難度外征レギオン制服",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "渡邉茜",
            Name = "百合ヶ丘標準制服",
            Type = "特殊単体"
        },
        new Costume
        {
            Lily = "高須賀月詩",
            Name = "百合ヶ丘標準制服",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "森辰姫",
            Name = "百合ヶ丘標準制服",
            Type = "支援"
        },
        new Costume
        {
            Lily = "六角汐里",
            Name = "百合ヶ丘標準制服",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "六角汐里",
            Name = "水夕会試作隊服",
            Type = "通常単体"
        },
        new Costume
        {
            Lily = "秦祀",
            Name = "百合ヶ丘標準制服",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "秦祀",
            Name = "秦祀隊レギオン制服",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "伊東閑",
            Name = "百合ヶ丘標準制服",
            Type = "回復"
        },
        new Costume
        {
            Lily = "伊東閑",
            Name = "ディアフレンド",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "立原紗癒",
            Name = "ローエングリン隊服",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "相澤一葉",
            Name = "エレンスゲ標準制服",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "相澤一葉",
            Name = "エレンスゲオーダー",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "相澤一葉",
            Name = "マリンセーラー",
            Type = "回復"
        },
        new Costume
        {
            Lily = "相澤一葉",
            Name = "オブシダンスーツ",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "相澤一葉",
            Name = "リリティカルサマー",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "相澤一葉",
            Name = "ライトウィンド",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "相澤一葉",
            Name = "ガーディアンスーツ",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "相澤一葉",
            Name = "ブリッツアングリフ",
            Type = "支援"
        },
        new Costume
        {
            Lily = "相澤一葉",
            Name = "カオスナイトメア",
            Type = "特殊単体"
        },
        new Costume
        {
            Lily = "相澤一葉",
            Name = "りりふぇす!!フラワー",
            Type = "支援"
        },
        new Costume
        {
            Lily = "相澤一葉",
            Name = "イージスガード",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "相澤一葉",
            Name = "のんびりスタイル",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "相澤一葉",
            Name = "ナイトリキャプチャー",
            Type = "回復"
        },
        new Costume
        {
            Lily = "佐々木藍",
            Name = "エレンスゲ標準制服",
            Type = "通常単体"
        },
        new Costume
        {
            Lily = "佐々木藍",
            Name = "エレンスゲオーダー",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "佐々木藍",
            Name = "マリンセーラー",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "佐々木藍",
            Name = "オブシダンスーツ",
            Type = "回復"
        },
        new Costume
        {
            Lily = "佐々木藍",
            Name = "リリティカルサマー",
            Type = "支援"
        },
        new Costume
        {
            Lily = "佐々木藍",
            Name = "サマースタイル",
            Type = "特殊単体"
        },
        new Costume
        {
            Lily = "佐々木藍",
            Name = "ナイトルージュ",
            Type = "支援"
        },
        new Costume
        {
            Lily = "佐々木藍",
            Name = "カオスナイトメア",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "佐々木藍",
            Name = "ブリッツアングリフ",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "佐々木藍",
            Name = "ハッピーハロウィン",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "飯島恋花",
            Name = "エレンスゲ標準制服",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "飯島恋花",
            Name = "エレンスゲオーダー",
            Type = "特殊単体"
        },
        new Costume
        {
            Lily = "飯島恋花",
            Name = "マリンセーラー",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "飯島恋花",
            Name = "華麗なるエージェント",
            Type = "特殊単体"
        },
        new Costume
        {
            Lily = "飯島恋花",
            Name = "ガーディアンスーツ",
            Type = "回復"
        },
        new Costume
        {
            Lily = "飯島恋花",
            Name = "イージスガード",
            Type = "回復"
        },
        new Costume
        {
            Lily = "飯島恋花",
            Name = "ブリッツアングリフ",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "初鹿野瑤",
            Name = "エレンスゲ標準制服",
            Type = "特殊単体"
        },
        new Costume
        {
            Lily = "初鹿野瑤",
            Name = "エレンスゲオーダー",
            Type = "支援"
        },
        new Costume
        {
            Lily = "初鹿野瑤",
            Name = "デモリッシャーフォーム",
            Type = "回復"
        },
        new Costume
        {
            Lily = "初鹿野瑤",
            Name = "華麗なるエージェント",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "初鹿野瑤",
            Name = "ガーディアンスーツ",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "初鹿野瑤",
            Name = "リリティカルサマー",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "初鹿野瑤",
            Name = "ブリッツアングリフ",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "初鹿野瑤",
            Name = "マリンセーラー",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "初鹿野瑤",
            Name = "イージスガード",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "初鹿野瑤",
            Name = "ナイトリキャプチャー",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "初鹿野瑤",
            Name = "ハッピーハロウィン",
            Type = "支援"
        },
        new Costume
        {
            Lily = "芹沢千香瑠",
            Name = "エレンスゲ標準制服",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "芹沢千香瑠",
            Name = "エレンスゲオーダー",
            Type = "回復"
        },
        new Costume
        {
            Lily = "芹沢千香瑠",
            Name = "エレガントストライカー",
            Type = "支援"
        },
        new Costume
        {
            Lily = "芹沢千香瑠",
            Name = "オブシダンスーツ",
            Type = "通常単体"
        },
        new Costume
        {
            Lily = "芹沢千香瑠",
            Name = "華麗なるエージェント",
            Type = "支援"
        },
        new Costume
        {
            Lily = "芹沢千香瑠",
            Name = "ブリッツアングリフ",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "芹沢千香瑠",
            Name = "カオスナイトメア",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "芹沢千香瑠",
            Name = "リリティカルサマー",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "芹沢千香瑠",
            Name = "涼風の浴衣",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "芹沢千香瑠",
            Name = "ナイトリキャプチャー",
            Type = "支援"
        },
        new Costume
        {
            Lily = "芹沢千香瑠",
            Name = "ハッピーハロウィン",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "今叶星",
            Name = "神庭女子標準制服",
            Type = "特殊単体"
        },
        new Costume
        {
            Lily = "今叶星",
            Name = "フローラルクインテット",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "今叶星",
            Name = "マルチカラードフラワー",
            Type = "支援"
        },
        new Costume
        {
            Lily = "今叶星",
            Name = "プリンセスナイト",
            Type = "通常単体"
        },
        new Costume
        {
            Lily = "今叶星",
            Name = "リリティカルサマー",
            Type = "回復"
        },
        new Costume
        {
            Lily = "今叶星",
            Name = "涼風の浴衣",
            Type = "特殊単体"
        },
        new Costume
        {
            Lily = "今叶星",
            Name = "ライトウィンド",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "今叶星",
            Name = "モンスターハロウィン",
            Type = "特殊単体"
        },
        new Costume
        {
            Lily = "今叶星",
            Name = "ブリッツアングリフ",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "今叶星",
            Name = "りりふぇす!!フラワー",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "今叶星",
            Name = "フラワーフレグランス",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "宮川高嶺",
            Name = "神庭女子標準制服",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "宮川高嶺",
            Name = "フローラルクインテット",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "宮川高嶺",
            Name = "プリンセスナイト",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "宮川高嶺",
            Name = "涼風の浴衣",
            Type = "回復"
        },
        new Costume
        {
            Lily = "宮川高嶺",
            Name = "モンスターハロウィン",
            Type = "支援"
        },
        new Costume
        {
            Lily = "宮川高嶺",
            Name = "マルチカラードフラワー",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "宮川高嶺",
            Name = "ブリッツアングリフ",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "宮川高嶺",
            Name = "フラワーフレグランス",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "土岐紅巴",
            Name = "神庭女子標準制服",
            Type = "通常単体"
        },
        new Costume
        {
            Lily = "土岐紅巴",
            Name = "フローラルクインテット",
            Type = "支援"
        },
        new Costume
        {
            Lily = "土岐紅巴",
            Name = "涼風の浴衣",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "土岐紅巴",
            Name = "マルチカラードフラワー",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "土岐紅巴",
            Name = "ビューティフルワールド",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "土岐紅巴",
            Name = "リリティカルサマー",
            Type = "回復"
        },
        new Costume
        {
            Lily = "土岐紅巴",
            Name = "ブリッツアングリフ",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "丹羽灯莉",
            Name = "神庭女子標準制服",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "丹羽灯莉",
            Name = "フローラルクインテット",
            Type = "回復"
        },
        new Costume
        {
            Lily = "丹羽灯莉",
            Name = "マルチカラードフラワー",
            Type = "通常単体"
        },
        new Costume
        {
            Lily = "丹羽灯莉",
            Name = "リリティカルサマー",
            Type = "特殊単体"
        },
        new Costume
        {
            Lily = "丹羽灯莉",
            Name = "モンスターハロウィン",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "丹羽灯莉",
            Name = "ファストブレイカー",
            Type = "支援"
        },
        new Costume
        {
            Lily = "丹羽灯莉",
            Name = "ビューティフルワールド",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "丹羽灯莉",
            Name = "ブリッツアングリフ",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "定盛姫歌",
            Name = "神庭女子標準制服",
            Type = "特殊単体"
        },
        new Costume
        {
            Lily = "定盛姫歌",
            Name = "フローラルクインテット",
            Type = "通常単体"
        },
        new Costume
        {
            Lily = "定盛姫歌",
            Name = "マルチカラードフラワー",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "定盛姫歌",
            Name = "プリンセスナイト",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "定盛姫歌",
            Name = "デモリッシャーフォーム",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "定盛姫歌",
            Name = "ビューティフルワールド",
            Type = "支援"
        },
        new Costume
        {
            Lily = "定盛姫歌",
            Name = "リリティカルサマー",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "定盛姫歌",
            Name = "フラワーフレグランス",
            Type = "通常単体"
        },
        new Costume
        {
            Lily = "定盛姫歌",
            Name = "ブリッツアングリフ",
            Type = "通常単体"
        },
        new Costume
        {
            Lily = "船田純",
            Name = "御台場女学校制服",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "船田初",
            Name = "御台場女学校制服",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "川村楪",
            Name = "御台場女学校制服",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "月岡椛",
            Name = "御台場女学校制服",
            Type = "回復"
        },
        new Costume
        {
            Lily = "藤田槿",
            Name = "御台場女学校制服",
            Type = "特殊範囲"
        },
        new Costume
        {
            Lily = "来夢",
            Name = "私立ルドビコ女学院制服",
            Type = "妨害"
        },
        new Costume
        {
            Lily = "幸恵",
            Name = "私立ルドビコ女学院制服",
            Type = "特殊単体"
        },
        new Costume
        {
            Lily = "百合亜",
            Name = "私立ルドビコ女学院制服",
            Type = "支援"
        },
        new Costume
        {
            Lily = "聖恋",
            Name = "私立ルドビコ女学院制服",
            Type = "通常範囲"
        },
        new Costume
        {
            Lily = "佳世",
            Name = "私立ルドビコ女学院制服",
            Type = "特殊範囲"
        }
    };
}