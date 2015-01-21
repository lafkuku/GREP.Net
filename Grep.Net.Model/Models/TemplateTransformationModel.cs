using System;
using System.Linq;
using Grep.Net.Entities;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using RazorEngine.Text;

namespace Grep.Net.Model.Models
{
    public class TemplateTransformationModel
    {
        public const string DEFAULT_TEMPLATE = @"@using Grep.Net.Entities
Title: SecBug - @Model.Issue.Name
Severity: @Model.Issue.Severity
@if(Model.Issue.Classification != null){
@:Classification: @Model.Issue.Classification.Name - @Model.Issue.Classification.ReferenceUrl
}
@if(!String.IsNullOrEmpty(Model.Issue.Description)){ 
@:Description: 
   @: @Model.Issue.Description 
   @:
}
@if(!String.IsNullOrEmpty(Model.Issue.ExploitScenario)){ 
@:Exploit Scenario: 
   @: @Model.Issue.ExploitScenario
   @:
}
@if(!String.IsNullOrEmpty(Model.Issue.Recommendation)){ 
@:Recommendation: 
   @: @Model.Issue.Recommendation   
   @:
}
@if(!String.IsNullOrEmpty(Model.Issue.References)){ 
@:References: 
   @: @Model.Issue.References   
   @:
}
   
Instances:

@foreach(MatchInfo mi in Model.MatchInfos){
  @: ************************************************************************************************************ 
  @:      File Name: @Raw(mi.FileInfo.FullName + "":"" + mi.LineNumber)
  @:           Line: @mi.Line
  if(!String.IsNullOrEmpty(mi.Pattern.Recommendation)){ 
  @: Recommendation: @mi.Pattern.Recommendation
  }
  if(!String.IsNullOrEmpty(mi.Pattern.ReferenceUrl)){ 
  @:        Ref Url: @mi.Pattern.ReferenceUrl
  }
  @: ************************************************************************************************************
}
";

        public TemplateService TemplateService { get; set; }

        public TemplateTransformationModel()
        {
            TemplateServiceConfiguration config = new TemplateServiceConfiguration();
            config.EncodedStringFactory = new MyEncodedStringFactory();
            this.TemplateService = new TemplateService(config);
        }

        public string Transform(Bug b)
        {
            //Because the Matches must come from the same "issue" it doesnt really matter which one we pull the template from. 
            if (b.MatchInfos.Count > 0)
            {
                if (b.Template == null)
                    throw new ArgumentException("Template is null");

                if (!string.IsNullOrWhiteSpace(b.Template.RawTemplate))
                {
                    string output = this.TemplateService.Parse(b.Template.RawTemplate, b, null, "Bug");
                    return output;
                }
            }

            return "";
        }
    }

    public class CustomCodeInspector : RazorEngine.Compilation.Inspectors.ICodeInspector
    {
        public void Inspect(System.CodeDom.CodeCompileUnit unit, System.CodeDom.CodeNamespace ns, System.CodeDom.CodeTypeDeclaration type, System.CodeDom.CodeMemberMethod executeMethod)
        {
            throw new NotImplementedException();
        }
    }

    public class MyEncodedStringFactory : IEncodedStringFactory
    {
        public IEncodedString CreateEncodedString(object value)
        {
            return new RawString(value.ToString()); 
        }

        public IEncodedString CreateEncodedString(string value)
        {
            return new RawString(value);
        }
    }
}