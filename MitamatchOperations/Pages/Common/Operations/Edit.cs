namespace Mitama.Pages.Common.Operations;

internal abstract record Edit;
internal record class Undo : Edit;
internal record class Redo : Edit;
