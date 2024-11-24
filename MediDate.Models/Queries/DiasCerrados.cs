using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediDate.Models.Queries
{
    public class DiasCerrados : BaseQuery
    {
        public DiasCerrados() : base() { }

        public BaseResult Create(DiaCerrado diaCerrado)
        {

            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<BaseResult>(
                    "sp_diasCerrados 1,'',@IdMedico,@Fecha1,@Fecha2",
                    new { diaCerrado.IdMedico, diaCerrado.Fecha1, diaCerrado.Fecha2 });
            }
        }

        public BaseResult Delete(int IdDiaCerrado)
        {

            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<BaseResult>(
                    "sp_diasCerrados 2,@IdDiaCerrado",
                    new { IdDiaCerrado });
            }
        }

        public BaseResult Edit(DiaCerrado diaCerrado)
        {

            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<BaseResult>(
                    "sp_diasCerrados 3,@IdDiaCerrado,@IdMedico,@Fecha1,@Fecha2",
                    new { diaCerrado.IdDiaCerrado, diaCerrado.IdMedico, diaCerrado.Fecha1, diaCerrado.Fecha2 });
            }
        }

        /// <summary>
        /// Busca un Dia Cerrado por IdDiaCerrado
        /// </summary>
        /// <param name="IdDiaCerrado"></param>
        /// <returns>Registro de DiaCerrado que conincida con IdDiaCerrado</returns>
        public DiaCerrado GetById(int IdDiaCerrado)
        {
            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<DiaCerrado>("sp_diasCerrados 4,@IdDiaCerrado,''", new { IdDiaCerrado });
            }
        }

        /// <summary>
        /// Mostrar todos los DiasCerrados de la tabla DiasCerrados
        /// </summary>
        /// <returns>Lista de DiasCerrados</returns>
        public List<DiaCerrado> GetAll()
        {
            var DiasCerrados = new List<DiaCerrado>();

            using (var db = GetConnection())
            {
                DiasCerrados = db.Query<DiaCerrado>("sp_diasCerrados 5,'','','',''").ToList();
            }

            return DiasCerrados;
        }

        public List<DiaCerrado> GetAllByIdMedico(int IdMedico)
        {
            var DiasCerrados = new List<DiaCerrado>();

            using (var db = GetConnection())
            {
                DiasCerrados = db.Query<DiaCerrado>("sp_diasCerrados 6,'',@IdMedico,'',''",new {IdMedico}).ToList();
            }

            return DiasCerrados;
        }
    }
}
