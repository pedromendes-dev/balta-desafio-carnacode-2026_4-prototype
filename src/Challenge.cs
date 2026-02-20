#:property PublishAot=false

// DESAFIO: Sistema de Templates de Documentos
// PROBLEMA: Um sistema de gerenciamento documental precisa criar novos documentos
// baseados em templates pré-configurados complexos (contratos, propostas, relatórios)
// O código atual recria objetos do zero, perdendo muito tempo em inicializações

using System;
using System.Collections.Generic;

namespace DesignPatternChallenge
{
    public interface IPrototype<out T>
    {
        T Clone();
    }

    public class DocumentTemplate : IPrototype<DocumentTemplate>
    {
        public string Title { get; set; }
        public string Category { get; set; }
        public List<Section> Sections { get; set; }
        public DocumentStyle Style { get; set; }
        public List<string> RequiredFields { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        public ApprovalWorkflow Workflow { get; set; }
        public List<string> Tags { get; set; }

        public DocumentTemplate()
        {
            Sections = new List<Section>();
            RequiredFields = new List<string>();
            Metadata = new Dictionary<string, string>();
            Tags = new List<string>();
            Title = string.Empty;
            Category = string.Empty;
            Style = new DocumentStyle();
            Workflow = new ApprovalWorkflow();
        }

        public DocumentTemplate Clone()
        {
            return new DocumentTemplate
            {
                Title = Title,
                Category = Category,
                Style = Style.Clone(),
                Workflow = Workflow.Clone(),
                Sections = Sections.ConvertAll(section => section.Clone()),
                RequiredFields = new List<string>(RequiredFields),
                Metadata = new Dictionary<string, string>(Metadata),
                Tags = new List<string>(Tags)
            };
        }
    }

    public class Section : IPrototype<Section>
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public bool IsEditable { get; set; }
        public List<string> Placeholders { get; set; }

        public Section()
        {
            Placeholders = new List<string>();
            Name = string.Empty;
            Content = string.Empty;
        }

        public Section Clone()
        {
            return new Section
            {
                Name = Name,
                Content = Content,
                IsEditable = IsEditable,
                Placeholders = new List<string>(Placeholders)
            };
        }
    }

    public class DocumentStyle : IPrototype<DocumentStyle>
    {
        public string FontFamily { get; set; }
        public int FontSize { get; set; }
        public string HeaderColor { get; set; }
        public string LogoUrl { get; set; }
        public Margins PageMargins { get; set; }

        public DocumentStyle()
        {
            FontFamily = "Arial";
            HeaderColor = "#000000";
            LogoUrl = string.Empty;
            PageMargins = new Margins();
        }

        public DocumentStyle Clone()
        {
            return new DocumentStyle
            {
                FontFamily = FontFamily,
                FontSize = FontSize,
                HeaderColor = HeaderColor,
                LogoUrl = LogoUrl,
                PageMargins = PageMargins.Clone()
            };
        }
    }

    public class Margins : IPrototype<Margins>
    {
        public int Top { get; set; }
        public int Bottom { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }

        public Margins Clone()
        {
            return new Margins
            {
                Top = Top,
                Bottom = Bottom,
                Left = Left,
                Right = Right
            };
        }
    }

    public class ApprovalWorkflow : IPrototype<ApprovalWorkflow>
    {
        public List<string> Approvers { get; set; }
        public int RequiredApprovals { get; set; }
        public int TimeoutDays { get; set; }

        public ApprovalWorkflow()
        {
            Approvers = new List<string>();
        }

        public ApprovalWorkflow Clone()
        {
            return new ApprovalWorkflow
            {
                Approvers = new List<string>(Approvers),
                RequiredApprovals = RequiredApprovals,
                TimeoutDays = TimeoutDays
            };
        }
    }

    public class DocumentService
    {
        private readonly Dictionary<string, DocumentTemplate> _prototypes = new(StringComparer.OrdinalIgnoreCase);

        public DocumentService()
        {
            RegisterPrototype("ServiceContract", CreateBaseServiceContractPrototype());
            RegisterPrototype("ConsultingContract", CreateBaseConsultingContractPrototype());
        }

        public void RegisterPrototype(string key, DocumentTemplate prototype)
        {
            _prototypes[key] = prototype;
        }

        public DocumentTemplate CreateFromPrototype(string key)
        {
            if (!_prototypes.TryGetValue(key, out var prototype))
                throw new ArgumentException($"Prototype '{key}' não encontrado");

            return prototype.Clone();
        }

        private DocumentTemplate CreateBaseServiceContractPrototype()
        {
            Console.WriteLine("Inicializando prototype de Contrato de Serviço (custo único)...");
            System.Threading.Thread.Sleep(100);

            var template = new DocumentTemplate
            {
                Title = "Contrato de Prestação de Serviços",
                Category = "Contratos",
                Style = new DocumentStyle
                {
                    FontFamily = "Arial",
                    FontSize = 12,
                    HeaderColor = "#003366",
                    LogoUrl = "https://company.com/logo.png",
                    PageMargins = new Margins { Top = 2, Bottom = 2, Left = 3, Right = 3 }
                },
                Workflow = new ApprovalWorkflow
                {
                    RequiredApprovals = 2,
                    TimeoutDays = 5
                }
            };

            template.Workflow.Approvers.Add("gerente@empresa.com");
            template.Workflow.Approvers.Add("juridico@empresa.com");

            template.Sections.Add(new Section
            {
                Name = "Cláusula 1 - Objeto",
                Content = "O presente contrato tem por objeto...",
                IsEditable = true
            });
            template.Sections.Add(new Section
            {
                Name = "Cláusula 2 - Prazo",
                Content = "O prazo de vigência será de...",
                IsEditable = true
            });
            template.Sections.Add(new Section
            {
                Name = "Cláusula 3 - Valor",
                Content = "O valor total do contrato é de...",
                IsEditable = true
            });

            template.RequiredFields.Add("NomeCliente");
            template.RequiredFields.Add("CPF");
            template.RequiredFields.Add("Endereco");

            template.Tags.Add("contrato");
            template.Tags.Add("servicos");

            template.Metadata["Versao"] = "1.0";
            template.Metadata["Departamento"] = "Comercial";
            template.Metadata["UltimaRevisao"] = DateTime.Now.ToString();

            return template;
        }

        private DocumentTemplate CreateBaseConsultingContractPrototype()
        {
            Console.WriteLine("Inicializando prototype de Contrato de Consultoria...");
            var template = CreateBaseServiceContractPrototype().Clone();
            template.Title = "Contrato de Consultoria";
            template.Tags.Remove("servicos");
            template.Tags.Add("consultoria");
            template.Sections[0].Content = "O presente contrato de consultoria tem por objeto...";
            template.Sections.RemoveAt(2);
            template.Metadata["UltimaRevisao"] = DateTime.Now.ToString();
            return template;
        }

        public void DisplayTemplate(DocumentTemplate template)
        {
            Console.WriteLine($"\n=== {template.Title} ===");
            Console.WriteLine($"Categoria: {template.Category}");
            Console.WriteLine($"Seções: {template.Sections.Count}");
            Console.WriteLine($"Campos obrigatórios: {string.Join(", ", template.RequiredFields)}");
            Console.WriteLine($"Aprovadores: {string.Join(", ", template.Workflow.Approvers)}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Sistema de Templates de Documentos ===\n");

            var service = new DocumentService();

            Console.WriteLine("Criando 5 contratos por clone do prototype...");
            var startTime = DateTime.Now;
            
            for (int i = 1; i <= 5; i++)
            {
                var contract = service.CreateFromPrototype("ServiceContract");
                contract.Title = $"Contrato #{i} - Cliente {i}";
                contract.Metadata["ClienteId"] = i.ToString();
            }
            
            var elapsed = (DateTime.Now - startTime).TotalMilliseconds;
            Console.WriteLine($"Tempo total: {elapsed}ms\n");

            var consultingContract = service.CreateFromPrototype("ConsultingContract");
            consultingContract.Metadata["Projeto"] = "Transformação Digital";
            service.DisplayTemplate(consultingContract);
        }
    }
}
