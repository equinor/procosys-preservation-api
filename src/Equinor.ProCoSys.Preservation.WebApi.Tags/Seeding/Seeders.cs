﻿using System;
using System.Collections.Generic;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ModeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.PersonAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.RequirementTypeAggregate;
using Equinor.ProCoSys.Preservation.Domain.AggregateModels.ResponsibleAggregate;

namespace Equinor.ProCoSys.Preservation.WebApi.Tags.Seeding
{
    public static class Seeders
    {
        public static void AddUsers(this IPersonRepository personRepository, int entryCount)
        {
            for (var i = 0; i < entryCount; i++)
            {
                var user = new Person(Guid.NewGuid(), $"Firstname-{i}", $"Lastname-{i}");
                personRepository.Add(user);
            }
        }

        public static void AddResponsibles(this IResponsibleRepository responsibleRepository, int entryCount, string plant)
        {
            for (var i = 0; i < entryCount; i++)
            {
                var responsible = new Responsible(plant, $"ResponsibleCode-{i}", $"ResponsibleDesc-{i}");
                responsibleRepository.Add(responsible);
            }
        }

        public static void AddModes(this IModeRepository modeRepository, int entryCount, string plant)
        {
            for (var i = 0; i < entryCount; i++)
            {
                var mode = new Mode(plant, $"Mode-{i}", false);
                modeRepository.Add(mode);
            }
        }

        public static void AddJourneys(this IJourneyRepository journeyRepository, int entryCount, string plant, List<Mode> modes, List<Responsible> responsibles)
        {
            var rand = new Random();

            for (var i = 0; i < entryCount; i++)
            {
                var journey = new Journey(plant, $"Journey-{i}");
                var step = new Step(plant, $"Step-{i}", modes[rand.Next(modes.Count)], responsibles[rand.Next(responsibles.Count)]);
                journey.AddStep(step);
                journeyRepository.Add(journey);
            }
        }

        public static void AddRequirementTypes(this IRequirementTypeRepository requirementTypeRepository, int entryCount, string plant)
        {
            for (var i = 0; i < entryCount; i++)
            {
                var requirementType = new RequirementType(plant, "Code", "Title", RequirementTypeIcon.Other, i);
                for (var j = 0; j < 5; j++)
                {
                    var requirementDefinition = new RequirementDefinition(plant, $"RequirementDefinition-{j}", 2, RequirementUsage.ForAll, j);
                    requirementType.AddRequirementDefinition(requirementDefinition);
                }
                requirementTypeRepository.Add(requirementType);
            }
        }

        public static void AddProjects(this IProjectRepository projectRepository, int entryCount, string plant)
        {
            for (var i = 0; i < entryCount; i++)
            {
                var project = new Project(plant, $"Project-{i}", "Decription", new Guid("aec8297b-b010-4c5d-91e0-7b1c8664ced8"));
                projectRepository.Add(project);
            }
        }

        public static void AddTags(this IEnumerable<Project> projects, int entryCountPrPoject, string plant, List<Step> steps, List<RequirementDefinition> requirementDefinitions)
        {
            var rand = new Random();

            foreach (var project in projects)
            {
                for (var i = 0; i < entryCountPrPoject; i++)
                {
                    var requirements = new List<TagRequirement>();
                    for (var j = 0; j < 5; j++)
                    {
                        var requirement = new TagRequirement(plant, 2,
                            requirementDefinitions[rand.Next(requirementDefinitions.Count)]);
                        requirements.Add(requirement);
                    }

                    var tag = new Tag(
                        plant, 
                        TagType.Standard,
                        Guid.NewGuid(),
                        $"TagNo-{i}",
                        "Description",
                        steps[rand.Next(steps.Count)],
                        requirements)
                    {
                        Calloff = "CallOffNo",
                        CommPkgNo = "CommPkgNo",
                        CommPkgProCoSysGuid = Guid.NewGuid(),
                        McPkgNo = "McPkgNo",
                        McPkgProCoSysGuid = Guid.NewGuid(),
                        PurchaseOrderNo = "PoNo",
                        Remark = "Remark",
                        StorageArea = "SA",
                        TagFunctionCode = "TFC"
                    };
                    tag.SetArea("AreaCode", "AreaDescription");
                    tag.SetDiscipline("DisciplineCode", "DisciplineDescription");

                    project.AddTag(tag);
                }
            }
        }
    }
}
