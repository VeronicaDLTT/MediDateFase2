using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediDate.Models.Queries
{
    public class Horarios: BaseQuery
    {
        public Horarios() : base(){}

        /// <summary>
        /// Mostrar todos los Horarios de la tabla Horarios
        /// </summary>
        /// <returns>Lista de Horarios</returns>
        public List<Horario> GetAll()
        {
            var horarios = new List<Horario>();

            using(var db = GetConnection())
            {
                horarios = db.Query<Horario>("sp_horarios 5,").ToList();
            }

            return horarios;
        }

        /// <summary>
        /// Busca un Horario por IdHorario
        /// </summary>
        /// <param name="IdHorario"></param>
        /// <returns>Registro de Horario que conincida con IdHorario</returns>
        public Horario GetById(int IdHorario)
        {
            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<Horario>("sp_horarios 4,@IdHorario,''", new { IdHorario });
            }
        }

        /// <summary>
        /// Busca un Horario por IdMedico
        /// </summary>
        /// <param name="IdMedico"></param>
        /// <returns>Registro de Horario que conincida con IdMedico</returns>
        public Horario GetByIdMedico(int IdMedico)
        {
            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<Horario>("sp_horarios 6,'',@IdMedico", new { IdMedico });
            }
        }

        public Horario GetHoraMinMaxByIdMedico(int IdMedico)
        {
            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<Horario>("sp_horarios 7,'',@IdMedico", new { IdMedico });
            }
        }

        public List<Horario> GetAllByIdMedico(int IdMedico)
        {
            var horarios = new List<Horario>();

            using (var db = GetConnection())
            {
                horarios = db.Query<Horario>("sp_horarios 6,'',@IdMedico", new { IdMedico }).ToList();
            }

            return horarios;
        }

        public BaseResult Create(int IdMedico, int Dia, TimeSpan HoraInicio, TimeSpan HoraFin, bool Estado)
        {

            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<BaseResult>(
                    "sp_horarios 1,'',@IdMedico,@Dia,@HoraInicio,@HoraFin,@Estado",
                    new { IdMedico, Dia, HoraInicio, HoraFin, Estado });
            }
        }

        public BaseResult Edit(Horario horario, TimeSpan HoraInicio, TimeSpan HoraFin)
        {

            using (var db = GetConnection())
            {
                return db.QueryFirstOrDefault<BaseResult>(
                    "sp_horarios 3,@IdHorario,'',@Dia, @HoraInicio,@HoraFin, @Estado",
                    new { horario.IdHorario, horario.Dia, HoraInicio, HoraFin, horario.Estado });
            }
        }
    }
}
