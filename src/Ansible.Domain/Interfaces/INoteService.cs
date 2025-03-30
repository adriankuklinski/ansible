using Ansible.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ansible.Domain.Interfaces;

public interface INoteService
{
    Task<IEnumerable<Note>> GetAllNotesAsync();
    Task<Note?> GetNoteByIdAsync(Guid id);
    Task<Note> CreateNoteAsync(Note note);
    Task UpdateNoteAsync(Note note);
    Task DeleteNoteAsync(Guid id);
    Task<IEnumerable<Note>> SearchNotesAsync(string searchTerm);
    Task<IEnumerable<Note>> GetNotesByTagAsync(string tagName);
    Task<IEnumerable<Note>> GetLinkedNotesAsync(Guid noteId);
    Task<NoteLink> CreateLinkAsync(NoteLink link);
    Task DeleteLinkAsync(Guid linkId);
}