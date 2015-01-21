using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Text;
using System.Collections.Generic;
using Caliburn.Micro;


namespace Grep.Net.Entities
{
    [Serializable]
    [DataContract(Namespace = "")]
    [XmlRootAttribute("GrepResult", Namespace = "")]
    public class GrepResult : BaseEntity, IEntity
    {
        [DataMember]
        [XmlElement]
        public GrepContext GrepContext { get; set; }
          [DataMember]
        [XmlArray(ElementName = "MatchInfos")]
        public virtual BindableCollection<MatchInfo> MatchInfos { get; set; }
          [DataMember]
          [XmlElement]
        public String Status { get; set; }

        public GrepResult() : base()
        {
            MatchInfos = new BindableCollection<MatchInfo>();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
           
            sb.AppendLine(String.Format("Search Path: {0}", GrepContext.RootPath));

            //Remove the trailing , 
            sb.Remove(sb.Length - 1, 1);

            var items = MatchInfos.OrderBy(x => x.Pattern.PatternPackageId);
            foreach (var item in items)
            {
                sb.AppendLine(item.ToString());
            }
            return sb.ToString();
        }
    }
}