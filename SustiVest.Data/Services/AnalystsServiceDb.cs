using System;
using SustiVest.Data.Entities;
using SustiVest.Data.Services;
using SustiVest.Data.Security;
using SustiVest.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql.Internal.TypeHandlers.NetworkHandlers;

namespace SustiVest.Data.Services
{
    public class AnalystServiceDb : IAnalystsService
    {
        private readonly DatabaseContext ctx;

        public AnalystServiceDb(DatabaseContext ctx)
        {
            this.ctx = ctx;
        }

        public IList<Analysts> GetAnalysts()
        {
            return ctx.Analysts.ToList();
        }

        public Paged<Analysts> GetAnalysts(int page = 1, int size = 20, string orderBy = "Name", string direction = "asc")
        {
            var results = (orderBy.ToLower(), direction.ToLower()) switch
            {
                ("analystno", "asc") => ctx.Analysts.OrderBy(a => a.AnalystNo),
                ("analystno", "desc") => ctx.Analysts.OrderByDescending(a => a.AnalystNo),

                ("name", "asc") => ctx.Analysts.OrderBy(a => a.Name),
                ("name", "desc") => ctx.Analysts.OrderByDescending(a => a.Name),
                _ => ctx.Analysts.OrderBy(a => a.AnalystNo)
            };

            return results.ToPaged(page, size, orderBy, direction);
        }

        public Analysts GetAnalyst(int analystNo)
        {
            return ctx.Analysts
            .Include(an => an.Assessments)
            .FirstOrDefault(an => an.AnalystNo == analystNo);
        }

        public Analysts GetAnalystByName(string name)
        {
            return ctx.Analysts.FirstOrDefault(an => an.Name == name);
        }

        public Analysts GetAnalystByEmail(string email)
        {
            return ctx.Analysts.FirstOrDefault(an => an.Email == email);
        }
        public Analysts AddAnalyst(Analysts an)
        {
            // Check if an analyst with the same name or email exists
            var existsByName = GetAnalystByName(an.Name);
            var existsByEmail = GetAnalystByEmail(an.Email);

            if (existsByName != null || existsByEmail != null)
            {
                return null; // Analyst with the same name or email already exists
            }

            var analyst = new Analysts
            {
                Name = an.Name,
                Email = an.Email,
                PhoneNo = an.PhoneNo
            };

            ctx.Analysts.Add(analyst);
            ctx.SaveChanges();
            return analyst;
        }


        public Analysts UpdateAnalyst(Analysts updated)
        {
            var analyst = GetAnalyst(updated.AnalystNo);
            if (analyst == null)
            {
                return null;
            }

            // Check if an analyst with the same name or email exists
            var existsByName = GetAnalystByName(updated.Name);
            var existsByEmail = GetAnalystByEmail(updated.Email);

            if ((existsByName != null && existsByName.AnalystNo != updated.AnalystNo) ||
                (existsByEmail != null && existsByEmail.AnalystNo != updated.AnalystNo))
            {
                return null; // Analyst with the same name or email already exists
            }

            analyst.Email = updated.Email;
            analyst.Name = updated.Name;
            analyst.PhoneNo = updated.PhoneNo;

            ctx.SaveChanges();
            return analyst;
        }
        public bool DeleteAnalyst(int analystNo)
        {
            var an = GetAnalyst(analystNo);
            if (an == null)
            {
                return false;
            }

            ctx.Analysts.Remove(an);
            ctx.SaveChanges();
            return true;
        }
    }
}