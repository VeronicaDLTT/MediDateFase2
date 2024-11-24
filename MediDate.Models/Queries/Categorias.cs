using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediDate.Models.Queries
{
    public class Categorias: BaseQuery
    {
        public Categorias() : base(){}

        public BaseResult Create(Categoria categoria)
        {

            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<BaseResult>(
                    "sp_categorias 1,'',@Descripcion",
                    new { categoria.DescripcionCategoria });
            }
        }

        public BaseResult Delete(int IdCategoria)
        {

            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<BaseResult>(
                    "sp_categorias 2,@IdCategoria",
                    new { IdCategoria });
            }
        }

        public BaseResult Edit(Categoria categoria)
        {

            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<BaseResult>(
                    "sp_categorias 3,@IdCategoria,@Descripcion",
                    new { categoria.IdCategoria, categoria.DescripcionCategoria });
            }
        }

        public Categoria GetById(int IdCategoria)
        {
            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<Categoria>("sp_categorias 4,@IdCategoria,''", new { IdCategoria });
            }
        }

        /// <summary>
        /// Mostrar todas los Categorias de la tabla Categorias
        /// </summary>
        /// <returns>Lista de Categorias</returns>
        public List<Categoria> GetAll()
        {
            var categorias = new List<Categoria>();

            using(var db = GetConnection())
            {
                categorias = db.Query<Categoria>("sp_categorias 5,'',''").ToList();
            }

            return categorias;
        }

    }
}
