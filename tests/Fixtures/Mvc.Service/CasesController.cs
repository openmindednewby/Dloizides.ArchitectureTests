using Microsoft.AspNetCore.Mvc;

namespace Mvc.Service;

public sealed class CasesController : ControllerBase
{
    public string Open([FromBody] OpenCaseRequest request) => request.Subject;

    public string Close(CloseCaseRequest request) => request.Reason;

    public string Search([FromQuery] CaseSearchQuery query) => query.Term;

    public int Get([FromRoute] int id, CaseLookup lookup) => id + lookup.CaseId;

    public string Now([FromServices] InjectedClock clock) => clock.Zone;

    [NonAction]
    public string Helper(HelperInput input) => input.Note;
}
