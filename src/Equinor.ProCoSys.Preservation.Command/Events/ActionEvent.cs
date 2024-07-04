using System;
using Equinor.ProCoSys.Preservation.MessageContracts;

namespace Equinor.ProCoSys.Preservation.Command.Events;

public class ActionEvent : IActionEventV1
{
    public ActionEvent(Guid guid, string plant, string projectName, Guid tagGuid, string title, string description, DateOnly? dueDate, bool overdue, DateOnly? closed)
    {
        Guid = guid;
        Plant = plant;
        ProjectName = projectName;
        TagGuid = tagGuid;
        Title = title;
        Description = description;
        DueDate = dueDate;
        Overdue = overdue;
        Closed = closed;
    }

    public Guid Guid { get; init; }

    public string Plant { get; init; }
    public string ProjectName { get; init; }
    public Guid TagGuid { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public DateOnly? DueDate { get; init; }
    public bool Overdue { get; init; }
    public DateOnly? Closed { get; init; }
}
