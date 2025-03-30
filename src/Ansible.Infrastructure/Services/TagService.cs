using Ansible.Domain.Interfaces;
using Ansible.Domain.Models;
using Ansible.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ansible.Infrastructure.Services;

public class TagService : ITagService
{
    private readonly AnsibleDbContext _dbContext;
    private readonly ILogger<TagService> _logger;

    public TagService(AnsibleDbContext dbContext, ILogger<TagService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IEnumerable<Tag>> GetAllTagsAsync()
    {
        _logger.LogInformation("Getting all tags");
        return await _dbContext.Tags.ToListAsync();
    }

    public async Task<Tag?> GetTagByIdAsync(Guid id)
    {
        _logger.LogInformation("Getting tag {TagId}", id);
        return await _dbContext.Tags.FindAsync(id);
    }

    public async Task<Tag> CreateTagAsync(Tag tag)
    {
        _logger.LogInformation("Creating new tag: {TagName}", tag.Name);
        
        tag.Id = Guid.NewGuid();
        
        await _dbContext.Tags.AddAsync(tag);
        await _dbContext.SaveChangesAsync();
        
        return tag;
    }

    public async Task UpdateTagAsync(Tag tag)
    {
        _logger.LogInformation("Updating tag {TagId}", tag.Id);
        
        var existingTag = await _dbContext.Tags.FindAsync(tag.Id);
        if (existingTag == null)
        {
            throw new KeyNotFoundException($"Tag with ID {tag.Id} not found");
        }
        
        existingTag.Name = tag.Name;
        existingTag.Color = tag.Color;
        
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteTagAsync(Guid id)
    {
        _logger.LogInformation("Deleting tag {TagId}", id);
        
        var tag = await _dbContext.Tags.FindAsync(id);
        if (tag == null)
        {
            throw new KeyNotFoundException($"Tag with ID {id} not found");
        }
        
        _dbContext.Tags.Remove(tag);
        await _dbContext.SaveChangesAsync();
    }
}