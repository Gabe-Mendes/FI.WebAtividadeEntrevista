using FI.AtividadeEntrevista.BLL;
using WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FI.AtividadeEntrevista.DML;
using System.Reflection;
using System.Web.Services.Description;

namespace WebAtividadeEntrevista.Controllers
{
    public class ClienteController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Incluir()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Incluir(ClienteModel model)
        {
            BoCliente bo = new BoCliente();

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {

                var CPFnumerico = model.CPF.Replace(".", "").Replace("-", "");

                if (!ValidarCpf(model.CPF))
                {
                    Response.StatusCode = 400;
                    return Json(string.Join(Environment.NewLine, "CPF inválido. Não foi possível realizar a inclusão do cliente."));
                }

                model.Id = bo.Incluir(new Cliente()
                {
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone,
                    CPF = CPFnumerico
                });

                return Json("Cadastro efetuado com sucesso");
            }
        }

        [HttpPost]
        public JsonResult Alterar(ClienteModel model)
        {
            BoCliente bo = new BoCliente();

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else
            {
                var CPFnumerico = model.CPF.Replace(".", "").Replace("-", "");

                if (!ValidarCpf(model.CPF))
                {
                    Response.StatusCode = 400;
                    return Json(string.Join(Environment.NewLine, "CPF inválido. Não foi possível realizar a alteração do cliente."));
                }

                bo.Alterar(new Cliente()
                {
                    Id = model.Id,
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone,
                    CPF = CPFnumerico
                });

                return Json("Cadastro alterado com sucesso");
            }
        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            BoCliente bo = new BoCliente();
            Cliente cliente = bo.Consultar(id);
            Models.ClienteModel model = null;

            if (cliente != null)
            {
                model = new ClienteModel()
                {
                    Id = cliente.Id,
                    CEP = cliente.CEP,
                    Cidade = cliente.Cidade,
                    Email = cliente.Email,
                    Estado = cliente.Estado,
                    Logradouro = cliente.Logradouro,
                    Nacionalidade = cliente.Nacionalidade,
                    Nome = cliente.Nome,
                    Sobrenome = cliente.Sobrenome,
                    Telefone = cliente.Telefone,
                    CPF = cliente.CPF
                };
            }
            return View(model);
        }

        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);

                //Return result to jTable
                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        
        [HttpPost]
        public JsonResult IncluirBeneficiario(long idCliente, string cpf, string nome)
        {
            BoBeneficiario bo = new BoBeneficiario();

            var CPFnumerico = cpf.Replace(".", "").Replace("-", "");

            if (!ValidarCpf(cpf))
            {
                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, "CPF inválido. Não foi possível realizar a inclusão do cliente."));
            }

            // Verifica existencia deste CPF no sistema
            if (bo.VerificarExistenciaBeneficiario(CPFnumerico))
            {
                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, "CPF inválido. Já esta em uso como beneficiário."));
            }

            bo.Incluir(new Beneficiario()
            {
                IdCliente = idCliente,
                Nome = nome,
                CPF = CPFnumerico
            });

            return Json(new { success = true });
        }
        
        public JsonResult BeneficiarioList(long clienteId)
        {
            try
            {
                List<Beneficiario> beneficiarios = new BoBeneficiario().Listar(clienteId);
                
                //Return result to jTable
                return Json(new { Result = "OK", Records = beneficiarios });
            }
            catch (Exception ex) 
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult ExcluiBeneficiario(long id)
        {
            try
            {
                BoBeneficiario bo = new BoBeneficiario();
                bo.Excluir(id);
                return Json(new { success = true });
            }
            catch(Exception)
            {
                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, "Falha no processo de exclusão do beneficiário."));
            }
        }
        [HttpGet]
        public JsonResult ConsultarBeneficiario(long idBeneficiario)
        {
            try
            {
                Beneficiario beneficiario = new BoBeneficiario().Consultar(idBeneficiario);
                return Json(new { success = true, data = beneficiario }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { success = false, Message = ex.Message });
            }            
        }

        [HttpPost]
        public JsonResult AlterarBeneficiario(long id, long idCliente, string cpf, string nome)
        {
            BoBeneficiario bo = new BoBeneficiario();
            
            var CPFnumerico = cpf.Replace(".", "").Replace("-", "");

            if (!ValidarCpf(cpf))
            {
                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, "CPF inválido. Não foi possível realizar a alteração do cliente."));
            }

            bo.Alterar(new Beneficiario()
            {
                Id = id,                    
                Nome = nome,
                CPF = CPFnumerico,
                IdCliente = idCliente
            });

            return Json(new { success = true });            
        }

        public static bool ValidarCpf(string cpf)
        {
            cpf = cpf.Replace(".", "").Replace("-", "");

            if (cpf.Length != 11)
                return false;

            if (new string(cpf[0], cpf.Length) == cpf)
                return false;

            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf;
            string digito;
            int soma;
            int resto;

            tempCpf = cpf.Substring(0, 9);
            soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();
            return cpf.EndsWith(digito);
        }
    }
}