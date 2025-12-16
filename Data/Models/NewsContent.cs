using ContentVersionsPOC.Data.Attributes;

namespace ContentVersionsPOC.Data.Models
{
    public class NewsContent : ContentVersion
    {
        [EditableProperty]
        public required string Heading { get; set; }

        [EditableProperty]
        public string? Lead { get; set; }

        [EditableProperty]
        public required string Text { get; set; }
    }
}
