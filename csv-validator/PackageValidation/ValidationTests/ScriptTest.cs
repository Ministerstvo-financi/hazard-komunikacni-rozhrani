using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Text;
using ValidationPilotServices.Infrastructure;
using ValidationPilotServices.Infrastructure.Enums;
using Xunit;
using Xunit.Abstractions;
using PackageValidation;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ValidationPilotTests
{

    public class Globals {
        public IDictionary<string,string> Context { get; set;}
    }

    public class ScriptTest : CoreTest
    {
        public ScriptTest(ITestOutputHelper output) : base(output)
        {
        }


        [Fact]
        public void DoScriptTest()
        {
            try
            {
                var scriptText=@"
                    Console.WriteLine($""xxx: {Context[""xxx""]}"");
                ";


                dynamic csvLine = new ExpandoObject();
                csvLine.FileName = "provozovatel.csv";
                csvLine.HraDruh = "B";
                csvLine.Model = "V";
                csvLine.Prihlasen = "2019-02-05T12:35:35.3+01:00";
                
                var globals = new Globals {
                    Context=new Dictionary<string,string>()
                };
                globals.Context["xxx"]="ahoj";
                var script = CSharpScript.Create(scriptText, GetScriptOptions(), globals.GetType());
                var scriptRunner = script.CreateDelegate();
                //scriptRunner.Invoke().Wait();
                script.Compile();
                script.RunAsync(globals);
            }
            catch (Microsoft.CodeAnalysis.Scripting.CompilationErrorException ex)
            {
                Console.WriteLine(string.Join(Environment.NewLine, ex.Diagnostics));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
            }
        }

        private ScriptOptions GetScriptOptions()
        {
            return ScriptOptions.Default
                .WithReferences(typeof(System.Diagnostics.Process).Assembly,            //System Assembly
                                typeof(System.Dynamic.DynamicObject).Assembly,          //System.Core Assembly
                                typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly,
                                this.GetType().Assembly )
                .WithImports(new[] {
                    "System",
                    "System.IO",
                    "System.Collections.Generic",
                    "System.Diagnostics",
                    "System.Dynamic",
                    "System.Linq",
                    "System.Linq.Expressions",
                    "System.Text",
                    "System.Threading.Tasks"
                });
        }
    }
}
