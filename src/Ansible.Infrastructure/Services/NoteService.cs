using Ansible.Domain.Interfaces;
using Ansible.Domain.Models;
using Ansible.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ansible.Infrastructure.Services;

public class NoteService : INoteService
{
    private readonly AnsibleDbContext _dbContext;
    private readonly ILogger<NoteService> _logger;

    public NoteService(AnsibleDbContext dbContext, ILogger<NoteService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IEnumerable<Note>> GetAllNotesAsync()
    {
        _logger.LogInformation("Getting all notes");
        return await _dbContext.Notes
            .Include(n => n.Tags)
            .Include(n => n.Links)
            .ToListAsync();
    }

    public async Task<Note?> GetNoteByIdAsync(Guid id)
    {
        _logger.LogInformation("Getting note {NoteId}", id);
        return await _dbContext.Notes
            .Include(n => n.Tags)
            .Include(n => n.Links)
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task<Note> CreateNoteAsync(Note note)
    {
        _logger.LogInformation("Creating new note");
        
        note.Id = Guid.NewGuid();
        note.CreatedAt = DateTime.UtcNow;
        
        await _dbContext.Notes.AddAsync(note);
        await _dbContext.SaveChangesAsync();
        
        return note;
    }

    public async Task UpdateNoteAsync(Note note)
    {
        _logger.LogInformation("Updating note {NoteId}", note.Id);
        
        var existingNote = await _dbContext.Notes
            .Include(n => n.Tags)
            .Include(n => n.Links)
            .FirstOrDefaultAsync(n => n.Id == note.Id);
            
        if (existingNote == null)
        {
            throw new KeyNotFoundException($"Note with ID {note.Id} not found");
        }
        
        existingNote.Title = note.Title;
        existingNote.Content = note.Content;
        existingNote.UpdatedAt = DateTime.UtcNow;
        existingNote.WorkItemId = note.WorkItemId;
        
        // Handle tags
        _dbContext.Entry(existingNote).Collection(n => n.Tags).Load();
        existingNote.Tags.Clear();
        foreach (var tag in note.Tags)
        {
            existingNote.Tags.Add(tag);
        }
        
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteNoteAsync(Guid id)
    {
        _logger.LogInformation("Deleting note {NoteId}", id);
        
        var note = await _dbContext.Notes.FindAsync(id);
        if (note == null)
        {
            throw new KeyNotFoundException($"Note with ID {id} not found");
        }
        
        // Delete related links
        var links = await _dbContext.NoteLinks
            .Where(l => l.SourceNoteId == id || l.TargetNoteId == id)
            .ToListAsync();
            
        _dbContext.NoteLinks.RemoveRange(links);
        _dbContext.Notes.Remove(note);
        
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<Note>> SearchNotesAsync(string searchTerm)
    {
        _logger.LogInformation("Searching notes with term: {SearchTerm}", searchTerm);
        
        return await _dbContext.Notes
            .Include(n => n.Tags)
            .Where(n => n.Title!.Contains(searchTerm) || n.Content!.Contains(searchTerm))
            .ToListAsync();
    }

    public async Task<IEnumerable<Note>> GetNotesByTagAsync(string tagName)
    {
        _logger.LogInformation("Getting notes by tag: {TagName}", tagName);
        
        return await _dbContext.Notes
            .Include(n => n.Tags)
            .Where(n => n.Tags.Any(t => t.Name == tagName))
            .ToListAsync();
    }

    public async Task<IEnumerable<Note>> GetLinkedNotesAsync(Guid noteId)
    {
        _logger.LogInformation("Getting linked notes for {NoteId}", noteId);
        
        var linkIds = await _dbContext.NoteLinks
            .Where(l => l.SourceNoteId == noteId)
            .Select(l => l.TargetNoteId)
            .ToListAsync();
            
        return await _dbContext.Notes
            .Include(n => n.Tags)
            .Where(n => linkIds.Contains(n.Id))
            .ToListAsync();
    }

    public async Task<NoteLink> CreateLinkAsync(NoteLink link)
    {
        _logger.LogInformation("Creating link between notes {SourceId} and {TargetId}", 
            link.SourceNoteId, link.TargetNoteId);
            
        link.Id = Guid.NewGuid();
        
        await _dbContext.NoteLinks.AddAsync(link);
        await _dbContext.SaveChangesAsync();
        
        return link;
    }

    public async Task DeleteLinkAsync(Guid linkId)
    {
        _logger.LogInformation("Deleting link {LinkId}", linkId);
        
        var link = await _dbContext.NoteLinks.FindAsync(linkId);
        if (link == null)
        {
            throw new KeyNotFoundException($"Link with ID {linkId} not found");
        }
        
        _dbContext.NoteLinks.Remove(link);
        await _dbContext.SaveChangesAsync();
    }
}