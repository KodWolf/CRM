using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Infrastructure.Data;
using CRM.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CRM.Application.Services
{
	public class StatusService : IStatusService
	{
		private readonly IGenericRepository<Status> _repository;
		private readonly ApplicationDbContext _context;

		public StatusService(IGenericRepository<Status> repository, ApplicationDbContext context)
		{
			_repository = repository;
			_context = context;
		}

		public async Task<IEnumerable<StatusDto>> GetAllAsync()
		{
			var statuses = await _repository.GetAllAsync();
			return statuses.Select(MapToDto);
		}

		public async Task<StatusDto?> GetByIdAsync(int id)
		{
			var status = await _repository.GetByIdAsync(id);
			return status == null ? null : MapToDto(status);
		}

		public async Task<StatusDto> CreateAsync(CreateStatusDto dto)
		{
			var status = new Status
			{
				Name = dto.Name,
				Description = dto.Description,
				Order = dto.Order,
				IsActive = true
			};

			await _repository.AddAsync(status);
			await _context.SaveChangesAsync();
			return MapToDto(status);
		}

		public async Task<StatusDto?> UpdateAsync(int id, CreateStatusDto dto)
		{
			var status = await _context.Statuses.FirstOrDefaultAsync(s => s.Id == id);
			if (status == null)
				return null;

			status.Name = dto.Name ?? status.Name;
			status.Description = dto.Description ?? status.Description;
			status.Order = dto.Order > 0 ? dto.Order : status.Order;

			_repository.Update(status);
			await _context.SaveChangesAsync();
			return MapToDto(status);
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var status = await _context.Statuses.FirstOrDefaultAsync(s => s.Id == id);
			if (status == null)
				return false;

			_repository.Delete(status);
			await _context.SaveChangesAsync();
			return true;
		}

		private StatusDto MapToDto(Status status)
		{
			return new StatusDto
			{
				Id = status.Id,
				Name = status.Name,
				Description = status.Description,
				Order = status.Order,
				IsActive = status.IsActive
			};
		}
	}

	public class PriorityService : IPriorityService
	{
		private readonly IGenericRepository<Priority> _repository;
		private readonly ApplicationDbContext _context;

		public PriorityService(IGenericRepository<Priority> repository, ApplicationDbContext context)
		{
			_repository = repository;
			_context = context;
		}

		public async Task<IEnumerable<PriorityDto>> GetAllAsync()
		{
			var priorities = await _repository.GetAllAsync();
			return priorities.Select(MapToDto);
		}

		public async Task<PriorityDto?> GetByIdAsync(int id)
		{
			var priority = await _repository.GetByIdAsync(id);
			return priority == null ? null : MapToDto(priority);
		}

		public async Task<PriorityDto> CreateAsync(CreatePriorityDto dto)
		{
			var priority = new Priority
			{
				Name = dto.Name,
				Level = dto.Level,
				ColorCode = dto.ColorCode,
				IsActive = true
			};

			await _repository.AddAsync(priority);
			await _context.SaveChangesAsync();
			return MapToDto(priority);
		}

		public async Task<PriorityDto?> UpdateAsync(int id, CreatePriorityDto dto)
		{
			var priority = await _context.Priorities.FirstOrDefaultAsync(p => p.Id == id);
			if (priority == null)
				return null;

			priority.Name = dto.Name ?? priority.Name;
			priority.Level = dto.Level > 0 ? dto.Level : priority.Level;
			priority.ColorCode = dto.ColorCode ?? priority.ColorCode;

			_repository.Update(priority);
			await _context.SaveChangesAsync();
			return MapToDto(priority);
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var priority = await _context.Priorities.FirstOrDefaultAsync(p => p.Id == id);
			if (priority == null)
				return false;

			_repository.Delete(priority);
			await _context.SaveChangesAsync();
			return true;
		}

		private PriorityDto MapToDto(Priority priority)
		{
			return new PriorityDto
			{
				Id = priority.Id,
				Name = priority.Name,
				Level = priority.Level,
				ColorCode = priority.ColorCode,
				IsActive = priority.IsActive
			};
		}
	}

	public class ChannelService : IChannelService
	{
		private readonly IGenericRepository<Channel> _repository;
		private readonly ApplicationDbContext _context;

		public ChannelService(IGenericRepository<Channel> repository, ApplicationDbContext context)
		{
			_repository = repository;
			_context = context;
		}

		public async Task<IEnumerable<ChannelDto>> GetAllAsync()
		{
			var channels = await _repository.GetAllAsync();
			return channels.Select(MapToDto);
		}

		public async Task<ChannelDto?> GetByIdAsync(int id)
		{
			var channel = await _repository.GetByIdAsync(id);
			return channel == null ? null : MapToDto(channel);
		}

		public async Task<ChannelDto> CreateAsync(CreateChannelDto dto)
		{
			var channel = new Channel
			{
				Name = dto.Name,
				Description = dto.Description,
				IsActive = true
			};

			await _repository.AddAsync(channel);
			await _context.SaveChangesAsync();
			return MapToDto(channel);
		}

		public async Task<ChannelDto?> UpdateAsync(int id, CreateChannelDto dto)
		{
			var channel = await _context.Channels.FirstOrDefaultAsync(c => c.Id == id);
			if (channel == null)
				return null;

			channel.Name = dto.Name ?? channel.Name;
			channel.Description = dto.Description ?? channel.Description;

			_repository.Update(channel);
			await _context.SaveChangesAsync();
			return MapToDto(channel);
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var channel = await _context.Channels.FirstOrDefaultAsync(c => c.Id == id);
			if (channel == null)
				return false;

			_repository.Delete(channel);
			await _context.SaveChangesAsync();
			return true;
		}

		private ChannelDto MapToDto(Channel channel)
		{
			return new ChannelDto
			{
				Id = channel.Id,
				Name = channel.Name,
				Description = channel.Description,
				IsActive = channel.IsActive
			};
		}
	}

	public class AttachmentService : IAttachmentService
	{
		private readonly IGenericRepository<Attachment> _repository;
		private readonly ApplicationDbContext _context;

		public AttachmentService(IGenericRepository<Attachment> repository, ApplicationDbContext context)
		{
			_repository = repository;
			_context = context;
		}

		public async Task<IEnumerable<AttachmentDto>> GetByTicketIdAsync(int ticketId)
		{
			var attachments = await _context.Attachments
				.Where(a => a.TicketId == ticketId)
				.ToListAsync();
			return attachments.Select(MapToDto);
		}

		public async Task<AttachmentDto?> GetByIdAsync(int id)
		{
			var attachment = await _repository.GetByIdAsync(id);
			return attachment == null ? null : MapToDto(attachment);
		}

		public async Task<AttachmentDto> CreateAsync(CreateAttachmentDto dto)
		{
			var attachment = new Attachment
			{
				FileName = dto.FileName,
				FilePath = dto.FilePath,
				FileSize = dto.FileSize,
				FileType = dto.FileType,
				TicketId = dto.TicketId,
				MessageId = dto.MessageId,
				UploadedAt = DateTime.UtcNow
			};

			await _repository.AddAsync(attachment);
			await _context.SaveChangesAsync();
			return MapToDto(attachment);
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var attachment = await _context.Attachments.FirstOrDefaultAsync(a => a.Id == id);
			if (attachment == null)
				return false;

			_repository.Delete(attachment);
			await _context.SaveChangesAsync();
			return true;
		}

		private AttachmentDto MapToDto(Attachment attachment)
		{
			return new AttachmentDto
			{
				Id = attachment.Id,
				FileName = attachment.FileName,
				FilePath = attachment.FilePath,
				FileSize = attachment.FileSize,
				FileType = attachment.FileType,
				UploadedAt = attachment.UploadedAt,
				TicketId = attachment.TicketId,
				MessageId = attachment.MessageId
			};
		}
	}
}
