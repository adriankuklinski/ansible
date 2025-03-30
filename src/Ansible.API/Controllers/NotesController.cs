using Ansible.Domain.Interfaces;
using Ansible.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ansible.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly INoteService _noteService;
    private readonly ILogger<NotesController> _logger;

    public NotesController(INoteService noteService, ILogger<NotesController> logger)
    {
        _noteService = noteService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Note>>> GetAllNotes()
    {
        try
        {
            var notes = await _noteService.GetAllNotesAsync();
            return Ok(notes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all notes");
            return StatusCode(500, "An error occurred while retrieving notes");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Note>> GetNote(Guid id)
    {
        try
        {
            var note = await _noteService.GetNoteByIdAsync(id);
            if (note == null)
            {
                return NotFound();
            }
            return Ok(note);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving note {NoteId}", id);
            return StatusCode(500, "An error occurred while retrieving the note");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Note>> CreateNote(Note note)
    {
        try
        {
            var createdNote = await _noteService.CreateNoteAsync(note);
            return CreatedAtAction(nameof(GetNote), new { id = createdNote.Id }, createdNote);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating note");
            return StatusCode(500, "An error occurred while creating the note");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateNote(Guid id, Note note)
    {
        if (id != note.Id)
        {
            return BadRequest("Note ID in URL must match the ID in the body");
        }

        try
        {
            await _noteService.UpdateNoteAsync(note);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating note {NoteId}", id);
            return StatusCode(500, "An error occurred while updating the note");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNote(Guid id)
    {
        try
        {
            await _noteService.DeleteNoteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting note {NoteId}", id);
            return StatusCode(500, "An error occurred while deleting the note");
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<Note>>> SearchNotes([FromQuery] string term)
    {
        try
        {
            var notes = await _noteService.SearchNotesAsync(term);
            return Ok(notes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching notes with term {SearchTerm}", term);
            return StatusCode(500, "An error occurred while searching notes");
        }
    }

    [HttpGet("bytag/{tagName}")]
    public async Task<ActionResult<IEnumerable<Note>>> GetNotesByTag(string tagName)
    {
        try
        {
            var notes = await _noteService.GetNotesByTagAsync(tagName);
            return Ok(notes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notes by tag {TagName}", tagName);
            return StatusCode(500, "An error occurred while retrieving notes by tag");
        }
    }

    [HttpGet("{id}/linked")]
    public async Task<ActionResult<IEnumerable<Note>>> GetLinkedNotes(Guid id)
    {
        try
        {
            var notes = await _noteService.GetLinkedNotesAsync(id);
            return Ok(notes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving linked notes for {NoteId}", id);
            return StatusCode(500, "An error occurred while retrieving linked notes");
        }
    }

    [HttpPost("link")]
    public async Task<ActionResult<NoteLink>> CreateLink(NoteLink link)
    {
        try
        {
            var createdLink = await _noteService.CreateLinkAsync(link);
            return Ok(createdLink);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating link between notes {SourceId} and {TargetId}", 
                link.SourceNoteId, link.TargetNoteId);
            return StatusCode(500, "An error occurred while creating the link");
        }
    }

    [HttpDelete("link/{linkId}")]
    public async Task<IActionResult> DeleteLink(Guid linkId)
    {
        try
        {
            await _noteService.DeleteLinkAsync(linkId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting link {LinkId}", linkId);
            return StatusCode(500, "An error occurred while deleting the link");
        }
    }
}