using Microsoft.EntityFrameworkCore;

namespace Library.Service;

public interface IConfigurationModeling
{
    void Configuration(ModelBuilder builder);
}