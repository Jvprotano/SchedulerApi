using AutoMapper;

using Bie.Api.Controllers.V1.Base;
using Bie.Api.DTOs.Request;
using Bie.Api.DTOs.Response;
using Bie.Business.Interfaces.Services;
using Bie.Business.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bie.Api.Controllers.V1;

// jv - verificar - remove in production
// [Authorize]
[Route("api/v{version:apiVersion}/[controller]")]
public class CompanyController : BaseController
{
    private readonly ICompanyService _companyService;
    private readonly IMapper _mapper;

    public CompanyController(ICompanyService companyService, IMapper mapper) : base()
    {
        _companyService = companyService;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CompanyRequestDto model)
    {
        try
        {
            var company = _mapper.Map<Company>(model);

            await _companyService.SaveAsync(company);

            return Ok(company);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put([FromBody] CompanyRequestDto model)
    {
        try
        {
            var company = _mapper.Map<Company>(model);

            await _companyService.SaveAsync(company);

            return Ok(company);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            await _companyService.TemporaryDeleteAsync(id);

            return Ok();
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
    [Route("Reactive")]
    [HttpGet]
    public async Task<IActionResult> Reactive(string id)
    {
        try
        {
            await _companyService.ReactiveAsync(id);
            return Ok();
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
    [HttpGet]
    public async Task<IActionResult> Get(string id)
    {
        var companyTest = new CompanyResponseDto()
        {
            Id = "abcId",
            Name = "Cartucho",
            Description = "Descrição"

        };
        return this.SuccessResponse(companyTest);

        var model = _mapper.Map<CompanyResponseDto>(await _companyService.GetByIdAsync(id));

        return Ok(model);
    }
    [HttpGet]
    [Route("GetByUserId")]
    public async Task<IActionResult> GetByUserId(string userId)
    {
        if (userId != null)
        {
            var companies = (await _companyService.GetCompaniesByUserAsync(userId)).ToList();
            var model = _mapper.Map<List<CompanyResponseDto>>(companies);
            return Ok(model);
        }

        return NotFound();
    }
}