using FI.AtividadeEntrevista.DML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FI.AtividadeEntrevista.BLL
{
    public class BoBeneficiario
    {
        public long Incluir(DML.Beneficiario beneficiario)
        {
            DAL.DaoBeneficiario bene = new DAL.DaoBeneficiario();
            return bene.Incluir(beneficiario);
        }

        public bool VerificarExistenciaBeneficiario(string CPF)
        {
            DAL.DaoBeneficiario bene = new DAL.DaoBeneficiario();
            return bene.VerificarExistenciaBeneficiario(CPF);
        }
        
        public List<DML.Beneficiario> Listar(long idCliente)
        {
            DAL.DaoBeneficiario bene = new DAL.DaoBeneficiario();
            return bene.Listar(idCliente);
        }

        public DML.Beneficiario Consultar(long idBeneficiario)
        {
            DAL.DaoBeneficiario bene = new DAL.DaoBeneficiario();
            return bene.Consultar(idBeneficiario);
        }

        public void Excluir(long id)
        {
            DAL.DaoBeneficiario bene = new DAL.DaoBeneficiario();
            bene.Excluir(id);
        }

        public void Alterar(DML.Beneficiario beneficiario)
        {
            DAL.DaoBeneficiario bene = new DAL.DaoBeneficiario();
            bene.Alterar(beneficiario);
        }
    }
}
