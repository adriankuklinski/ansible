using System;
using System.Collections.Generic;

namespace Ansible.Domain.Models;

public class Note
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public List<Tag> Tags { get; set; } = new();
    public List<NoteLink> Links { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? WorkItemId { get; set; } // Optional reference to Azure DevOps work item
}

public class Tag
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Color { get; set; }
}

public class NoteLink
{
    public Guid Id { get; set; }
    public Guid SourceNoteId { get; set; }
    public Guid TargetNoteId { get; set; }
    public string? Description { get; set; }
    public LinkType Type { get; set; }
}

public enum LinkType
{
    Reference,
    Supports,
    Contradicts,
    Elaborates,
    Example,
    Question,
    Answer
}