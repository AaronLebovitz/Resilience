using System;
using AppKit;
using Foundation;
using System.Collections.Generic;

namespace ResilienceClasses
{
    public class ComboBoxStringListDataSource : NSComboBoxDataSource
    {
        readonly List<string> source;

        public ComboBoxStringListDataSource(List<string> source)
        {
            this.source = source;
        }

        public override string CompletedString(NSComboBox comboBox, string uncompletedString)
        {
            return source.Find(n => n.StartsWith(uncompletedString, StringComparison.InvariantCultureIgnoreCase));
        }

        public override nint IndexOfItem(NSComboBox comboBox, string value)
        {
            return source.FindIndex(n => n.Equals(value, StringComparison.InvariantCultureIgnoreCase));
        }

        public override nint ItemCount(NSComboBox comboBox)
        {
            return source.Count;
        }

        public override NSObject ObjectValueForItem(NSComboBox comboBox, nint index)
        {
            return NSObject.FromObject(source[(int)index]);
        }

        public string Value(int index)
        {
            string str = null;
            if (index >= 0) str = source[index];
            return str;
        }
    }
}
