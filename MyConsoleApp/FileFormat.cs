using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;
using FileHelpers.Events;
namespace MyConsoleApp
{
    [IgnoreFirst]
    [IgnoreEmptyLines]
    [DelimitedRecord("|")]
    public class FileFormat : INotifyRead
    {

        [FieldHidden]
        public int LineNumber;

        public int ConstituentId;

       

        [FieldConverter(ConverterKind.Date, "MM/dd/yyyy")]
        public DateTime? BeginsOn;

        [FieldConverter(ConverterKind.Date, "MM/dd/yyyy")]
        public DateTime? EndsOn;

        public string InvolvementCode;

        public string Comment;

        public void BeforeRead(BeforeReadEventArgs e)
        {
        }

        public void AfterRead(AfterReadEventArgs e)
        {
            LineNumber = e.LineNumber;
        }
    }
}
