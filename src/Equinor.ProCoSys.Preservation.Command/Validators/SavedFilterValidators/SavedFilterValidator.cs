﻿using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Common.Misc;
using Equinor.ProCoSys.Common;

namespace Equinor.ProCoSys.Preservation.Command.Validators.SavedFilterValidators
{
    public class SavedFilterValidator : ISavedFilterValidator
    {
        private readonly IReadOnlyContext _context;
        private readonly ICurrentUserProvider _currentUserProvider;

        public SavedFilterValidator(
            IReadOnlyContext context,
            ICurrentUserProvider currentUserProvider)
        {
            _context = context;
            _currentUserProvider = currentUserProvider;
        }

        public async Task<bool> ExistsWithSameTitleForPersonInProjectAsync(string title, string projectName,
            CancellationToken token)
        {
            var currentUserOid = _currentUserProvider.GetCurrentUserOid();

            return await (from s in _context.QuerySet<SavedFilter>()
                join p in _context.QuerySet<Person>() on EF.Property<int>(s, "PersonId") equals p.Id
                join pr in _context.QuerySet<Project>() on s.ProjectId equals pr.Id
                where pr.Name == projectName
                      && p.Guid == currentUserOid
                      && s.Title == title
                select s).AnyAsync(token);
        }

        public async Task<bool> ExistsAnotherWithSameTitleForPersonInProjectAsync(int savedFilterId, string title,
            CancellationToken token)
        {
            var currentUserOid = _currentUserProvider.GetCurrentUserOid();

            var projectName = await (from s in _context.QuerySet<SavedFilter>()
                join p in _context.QuerySet<Person>() on EF.Property<int>(s, "PersonId") equals p.Id
                join pr in _context.QuerySet<Project>() on s.ProjectId equals pr.Id
                where s.Id == savedFilterId
                select pr.Name).SingleOrDefaultAsync(token);

            return await (from s in _context.QuerySet<SavedFilter>()
                join p in _context.QuerySet<Person>() on EF.Property<int>(s, "PersonId") equals p.Id
                join pr in _context.QuerySet<Project>() on s.ProjectId equals pr.Id
                where pr.Name == projectName
                      && p.Guid == currentUserOid
                      && s.Title == title
                      && s.Id != savedFilterId
                select s).AnyAsync(token);
        }

        public async Task<bool> ExistsAsync(int savedFilterId, CancellationToken token) =>
            await (from sf in _context.QuerySet<SavedFilter>()
                where sf.Id == savedFilterId
                select sf).AnyAsync(token);
    }
}
