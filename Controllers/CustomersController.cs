using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly string _connectionString;

    public CustomersController()
    {
        var ini = new INIReader("config.ini");
        var server = ini.Read("Connection", "Server");
        var db = ini.Read("Connection", "Database");
        var user = ini.Read("Connection", "User");
        var pass = ini.Read("Connection", "Password");
        _connectionString = $"Server={server};Database={db};User Id={user};Password={pass};TrustServerCertificate=True";
    }

    // ✅ GET: api/customers
    [HttpGet]
    public IActionResult GetCustomers()
    {
        var list = new List<CustomerCard>();
        using var con = new SqlConnection(_connectionString);
        con.Open();
        var cmd = new SqlCommand("SELECT * FROM CustomerCard", con);
        var rdr = cmd.ExecuteReader();
        while (rdr.Read())
        {
            list.Add(new CustomerCard
            {
                Id = Convert.ToInt32(rdr["Id"]),
                CustomerName = rdr["CustomerName"]?.ToString() ?? "",
                Phone = rdr["Phone"]?.ToString() ?? "",
                Email = rdr["Email"]?.ToString() ?? "",
                Address = rdr["Address"]?.ToString() ?? "",
                Balance = Convert.ToSingle(rdr["Balance"]),
                EntryDate = Convert.ToDateTime(rdr["EntryDate"])
            });
        }
        return Ok(list);
    }

    // ✅ POST: api/customers
    [HttpPost]
    public IActionResult AddCustomer([FromBody] CustomerCard c)
    {
        if (string.IsNullOrWhiteSpace(c.CustomerName))
            return BadRequest("الاسم مطلوب");

        using var con = new SqlConnection(_connectionString);
        con.Open();

        var cmd = new SqlCommand(@"
            INSERT INTO CustomerCard 
            (CustomerName, Phone, Email, Address, Balance, EntryDate) 
            VALUES 
            (@CustomerName, @Phone, @Email, @Address, @Balance, @EntryDate)", con);

        cmd.Parameters.AddWithValue("@CustomerName", c.CustomerName);
        cmd.Parameters.AddWithValue("@Phone", c.Phone ?? "");
        cmd.Parameters.AddWithValue("@Email", c.Email ?? "");
        cmd.Parameters.AddWithValue("@Address", c.Address ?? "");
        cmd.Parameters.AddWithValue("@Balance", c.Balance);
        cmd.Parameters.AddWithValue("@EntryDate", c.EntryDate);

        cmd.ExecuteNonQuery();
        return Ok("تمت إضافة العميل بنجاح");
    }

    // ✅ PUT: api/customers/{id}
    [HttpPut("{id}")]
    public IActionResult UpdateCustomer(int id, [FromBody] CustomerCard c)
    {
        if (string.IsNullOrWhiteSpace(c.CustomerName))
            return BadRequest("الاسم مطلوب");

        using var con = new SqlConnection(_connectionString);
        con.Open();

        var cmd = new SqlCommand(@"
            UPDATE CustomerCard SET 
                CustomerName = @CustomerName,
                Phone = @Phone,
                Email = @Email,
                Address = @Address,
                Balance = @Balance,
                EntryDate = @EntryDate
            WHERE Id = @Id", con);

        cmd.Parameters.AddWithValue("@Id", id);
        cmd.Parameters.AddWithValue("@CustomerName", c.CustomerName);
        cmd.Parameters.AddWithValue("@Phone", c.Phone ?? "");
        cmd.Parameters.AddWithValue("@Email", c.Email ?? "");
        cmd.Parameters.AddWithValue("@Address", c.Address ?? "");
        cmd.Parameters.AddWithValue("@Balance", c.Balance);
        cmd.Parameters.AddWithValue("@EntryDate", c.EntryDate);

        int rowsAffected = cmd.ExecuteNonQuery();
        if (rowsAffected == 0)
            return NotFound("لم يتم العثور على العميل");

        return Ok("تم تعديل بيانات العميل بنجاح");
    }

    // ✅ DELETE: api/customers/{id}
    [HttpDelete("{id}")]
    public IActionResult DeleteCustomer(int id)
    {
        using var con = new SqlConnection(_connectionString);
        con.Open();

        var cmd = new SqlCommand("DELETE FROM CustomerCard WHERE Id = @Id", con);
        cmd.Parameters.AddWithValue("@Id", id);

        int rowsAffected = cmd.ExecuteNonQuery();
        if (rowsAffected == 0)
            return NotFound("لم يتم العثور على العميل");

        return Ok("تم حذف العميل بنجاح");
    }
}
