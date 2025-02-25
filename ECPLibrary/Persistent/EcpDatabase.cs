using Microsoft.EntityFrameworkCore;

namespace ECPLibrary.Persistent;

public interface IEcpDatabase { }

public class EcpDatabase(DbContextOptions<EcpDatabase> options) : DbContext(options), IEcpDatabase;

public class EcpIdentityDatabase(DbContextOptions<EcpIdentityDatabase> options) : DbContext(options), IEcpDatabase;