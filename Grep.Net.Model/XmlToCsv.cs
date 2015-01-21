using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Grep.Net.Model.Types
{
    public class XmlToDelimitedRowColumn
    {
        public enum RowDelimit
        {
            Default,
            NewLine,
            Space,
            Ambescent,
            Dollar
        }
        public enum ColumnDelimit
        {
            Default,
            Comma,
            TabSpace,
            Percentage,
            OrSymbol,
            Dot
        }
        public enum DataArrange
        {
            Element,
            Attribute
        }

        public static RowDelimit RowDelimiter { set; get; }

        public static ColumnDelimit ColumnDelimiter { set; get; }

        public static void FetchRowSeparater(RowDelimit delimit, out string separater)
        {
            switch (delimit)
            {
                case RowDelimit.NewLine:
                case RowDelimit.Default:
                    separater = Environment.NewLine;
                    break;
                case RowDelimit.Space:
                    separater = " ";
                    break;
                case RowDelimit.Dollar:
                    separater = "$";
                    break;
                case RowDelimit.Ambescent:
                    separater = "&";
                    break;
                default:
                    separater = Environment.NewLine;
                    break;
            }
        }


        public static void FetchColumnSeparater(ColumnDelimit delimit, out string separater)
        {
            switch (delimit)
            {
                case ColumnDelimit.Comma:
                case ColumnDelimit.Default:
                    separater = ",";
                    break;
                case ColumnDelimit.Dot:
                    separater = ".";
                    break;
                case ColumnDelimit.OrSymbol:
                    separater = "|";
                    break;
                case ColumnDelimit.Percentage:
                    separater = "%";
                    break;
                case ColumnDelimit.TabSpace:
                    separater = "\t";
                    break;
                default:
                    separater = ",";
                    break;
            }
        }
        public static String Convert(string rawXml, string datatag, DataArrange arrange, RowDelimit rdelimit, ColumnDelimit cdelimit)
        {
            try
            {
                StringBuilder builder = new StringBuilder();

                XDocument doc = XDocument.Parse(rawXml);
                string Rowseparater = string.Empty;
                FetchRowSeparater(rdelimit, out Rowseparater);

                string Columnseparater = string.Empty;
                FetchColumnSeparater(cdelimit, out Columnseparater);

                foreach (XElement data in doc.Descendants(datatag))
                {
                    if (arrange == DataArrange.Element)
                    {
                        foreach (XElement innnerval in data.Elements())
                        {
                            builder.Append(innnerval.Value);
                            builder.Append(Columnseparater);
                        }
                    }
                    else
                    {
                        foreach (XAttribute innerval in data.Attributes())
                        {
                            builder.Append(innerval.Value);
                            builder.Append(Columnseparater);
                        }
                    }
                    builder.Append(Rowseparater);
                }

                return builder.ToString();
            }
            catch
            {
                throw;
            }
        }

        public static void Convert(string xmlfilepath, string csvpath, string datatag, DataArrange arrange, RowDelimit rdelimit, ColumnDelimit cdelimit)
        {
            try
            {
                StringBuilder builder = new StringBuilder();

                XDocument doc = XDocument.Load(xmlfilepath);
                string Rowseparater = string.Empty;
                FetchRowSeparater(rdelimit, out Rowseparater);

                string Columnseparater = string.Empty;
                FetchColumnSeparater(cdelimit, out Columnseparater);

                foreach (XElement data in doc.Descendants(datatag))
                {
                    if (arrange == DataArrange.Element)
                    {
                        foreach (XElement innnerval in data.Elements())
                        {
                            builder.Append(innnerval.Value);
                            builder.Append(Columnseparater);
                        }
                    }
                    else
                    {
                        foreach (XAttribute innerval in data.Attributes())
                        {
                            builder.Append(innerval.Value);
                            builder.Append(Columnseparater);
                        }
                    }
                    builder.Append(Rowseparater);
                }

                File.AppendAllText(csvpath, builder.ToString());
            }
            catch
            {
                throw;
            }
        }
    }
}
