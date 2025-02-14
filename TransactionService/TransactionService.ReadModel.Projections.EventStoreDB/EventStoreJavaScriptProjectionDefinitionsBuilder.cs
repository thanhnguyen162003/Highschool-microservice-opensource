using DStack.Projections.EventStoreDB;
using System;
using System.Collections.Generic;
using TransactionService.PL.Events;

namespace TransactionService.ReadModel.Projections.EventStoreDB;

public class EventStoreJavaScriptProjectionDefinitionsBuilder
{
    public static Dictionary<string, string> Build()
    {
        var ret = new Dictionary<string, string>();
        var defs = new List<EventStoreProjection>
        {
            EventStoreProjectionBuilder.BuildProjectionDefinition(CreateOrganizationNameCorrectionsProjectionParams())
        };
        foreach (var d in defs)
            ret.Add(d.Name, d.Source);
        return ret;
    }

    static EventStoreProjectionParameters CreateOrganizationNameCorrectionsProjectionParams()
    {
        const string ProjectionName = "OrganizationNameCorrectionsProjection";
        return new EventStoreProjectionParameters
        {
            Name = ProjectionName,
            SourceStreamNames = new List<string> { OrganizationProjection.StreamName },
            DestinationStreamName = "cp-OrganizationNameCorrections",
            EventsToInclude = new Type[] { typeof(OrganizationNameCorrected) }
        };
    }
}