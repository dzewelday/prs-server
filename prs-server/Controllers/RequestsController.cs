﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prs_server.Models;

namespace prs_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestsController : ControllerBase
    {
        private readonly PrsContext _context;

        public RequestsController(PrsContext context)
        {
            _context = context;
        }


        // GET api/Requests/review/id
        [HttpGet("review/{id}")]

        public async Task<ActionResult<IEnumerable<Request>>> GetReviews(int userId)
        {
            return await _context.Requests.Where(x => x.Status == "REVIEW" && x.UserId != userId ).ToListAsync();
        }


        // PUT: api/Requests/5/review
        [HttpPut("{id}/review")]
        public async Task<ActionResult<Request>> Review(Request request)
        {
            var req = await _context.Requests.FindAsync(request.Id);
            if (req == null)
            {
                return NotFound();
            }
            req.Status = (req.Total <= 50) ? "APPROVED" : "REVIEW";
            return request;
        }


        // PUT: api/Requests/5/approve
        [HttpPut("{id}/approve")]
        public async Task<IActionResult> Approve(Request request)
        {
            request.Status = "APPROVED";
            await _context.SaveChangesAsync();
            return NoContent();
           
        }

        // PUT: api/Requests/5/reject
        [HttpPut("{id}/reject")]
        public async Task<IActionResult> Reject(Request request)
        {
            request.Status = "REJECTED";
            await _context.SaveChangesAsync();
            return NoContent();

        }

        // GET: api/Requests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Request>>> GetRequests()
        {
            return await _context.Requests.Include(x => x.User).ToListAsync();
        }

        // GET: api/Requests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Request>> GetRequest(int id)
        {
            var request = await _context.Requests.Include(x => x.User)
                                                 .Include(x => x.RequestLines)
                                                 .SingleOrDefaultAsync(x => x.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            return request;
        }

        // PUT: api/Requests/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRequest(int id, Request request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            _context.Entry(request).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequestExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Requests
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Request>> PostRequest(Request request)
        {
            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRequest", new { id = request.Id }, request);
        }

        // DELETE: api/Requests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            _context.Requests.Remove(request);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RequestExists(int id)
        {
            return _context.Requests.Any(e => e.Id == id);
        }
    }
}
