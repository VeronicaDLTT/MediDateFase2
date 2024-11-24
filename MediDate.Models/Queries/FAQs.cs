using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediDate.Models.Queries
{
    public class FAQs: BaseQuery
    {
        public FAQs() : base(){}

        public BaseResult Create(FAQ faq)
        {

            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<BaseResult>(
                    "sp_FAQs 1,'',@IdCategoria,@DescripcionFAQ,@Respuesta,@Accion",
                    new { faq.IdCategoria, faq.DescripcionFAQ, faq.Respuesta, faq.Accion });
            }
        }

        public BaseResult Delete(int IdFAQ)
        {

            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<BaseResult>(
                    "sp_FAQs 2,@IdFAQ",
                    new { IdFAQ });
            }
        }

        public BaseResult Edit(FAQ faq)
        {

            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<BaseResult>(
                    "sp_FAQs 3,@IdFAQ,@IdCategoria,@DescripcionFAQ,@Respuesta,@Accion",
                    new { faq.IdFAQ, faq.IdCategoria, faq.DescripcionFAQ, faq.Respuesta, faq.Accion });
            }
        }

        public FAQ GetById(int IdFAQ)
        {
            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<FAQ>("sp_FAQs 4,@IdFAQ,''", new { IdFAQ });
            }
        }

        /// <summary>
        /// Mostrar todas las FAQs de la tabla FAQs
        /// </summary>
        /// <returns>Lista de FAQs</returns>
        public List<FAQ> GetAll()
        {
            var FAQs = new List<FAQ>();

            using(var db = GetConnection())
            {
                FAQs = db.Query<FAQ>("sp_FAQs 5,'',''").ToList();
            }

            return FAQs;
        }

        public List<FAQ> GetSearch(int IdCategoria, string Descripcion)
        {
            var FAQs = new List<FAQ>();

            using (var db = GetConnection())
            {

                //FAQs = db.Query<FAQ>("sp_FAQs 6,'',@IdCategoria,@Descripcion", new { IdCategoria, Descripcion }).ToList();
                
                //if ((Descripcion != "") || (Descripcion != null)) //Si Descripcion no esta vació, realiza la busqueda por Descripcion
                //{
                //    IdCategoria = 0;
                //}
                //else //Si Descripcion esta vació
                //{
                //    Descripcion = "";
                //}

                FAQs = db.Query<FAQ>("sp_FAQs 6,'',@IdCategoria,@Descripcion", new { IdCategoria, Descripcion }).ToList();

            }

            return FAQs;
        }

    }
}
