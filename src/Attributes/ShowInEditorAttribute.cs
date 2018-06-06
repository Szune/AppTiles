using System;
using System.Runtime.CompilerServices;

namespace AppTiles.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ShowInEditorAttribute : Attribute
    {
        public string DisplayText { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsAdvanced { get; set; }
        public bool IsBaseClass { get; set; }
        public ShowInEditorAttribute(bool isReadOnly = false, bool isAdvanced = false, bool isBaseClass = false, [CallerMemberName] string text = "")
        {
            DisplayText = text;
            IsReadOnly = isReadOnly;
            IsAdvanced = isAdvanced;
            IsBaseClass = isBaseClass;
        }
    }
}
