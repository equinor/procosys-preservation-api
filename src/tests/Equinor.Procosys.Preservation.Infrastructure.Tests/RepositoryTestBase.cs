using System.Collections.Generic;
using Equinor.Procosys.Preservation.Domain.AggregateModels.JourneyAggregate;
using Equinor.Procosys.Preservation.Domain.AggregateModels.ProjectAggregate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.Procosys.Preservation.Infrastructure.Tests
{
    [TestClass]
    public class RepositoryTestBase
    {
        protected const string TestPlant = "PCS$TESTPLANT";
        protected ContextHelper ContextHelper;
        
        [TestInitialize]
        public void RepositorySetup()
        {
            ContextHelper = new ContextHelper();
        }
    }
}
