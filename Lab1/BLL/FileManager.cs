using Lab1.DAL;
using Lab1.DAL.Entities;
using Lab1.Models;
using Microsoft.EntityFrameworkCore;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Security.Principal;

namespace Lab1.BLL;

public class FileManager
{
    private readonly AppDbContext context;

    public FileManager(AppDbContext context)
    {
        this.context = context;
    }

    public bool IsLocalUser(string login)
    {
        return WindowsIdentity.GetCurrent().Name == login;
    }

    public bool LoginExists(string login) => context.Users.Any(x => x.Login == login);

    public User RegisterUser(UserRegistraionModel registraionModel)
    {
        var user = new User()
        {
            IPAddress = registraionModel.IPAddress,
            Login = registraionModel.Login,
            PasswordHash = registraionModel.PasswordHash,
        };

        context.Users.Add(user);

        context.SaveChanges();

        return user;
    }

    public IQueryable<User> Users()
    {
        return context.Users.AsQueryable();
    }

    public void SetUserBlock(Guid userId, bool unblock = false)
    {
        var user = context.Users.First(x => x.Id == userId);

        user.IsActive = unblock;

        context.SaveChanges();
    }

    /// <summary>
    ///     Validate username and password combination    
    ///     <para>Following Windows Services must be up</para>
    ///     <para>LanmanServer; TCP/IP NetBIOS Helper</para>
    /// </summary>
    /// <param name="userName">
    ///     Fully formatted UserName.
    ///     In AD: Domain + Username
    ///     In Workgroup: Username or Local computer name + Username
    /// </param>
    /// <param name="securePassword"></param>
    /// <returns></returns>
    public static bool ValidateUsernameAndPassword(string userName, SecureString securePassword)
    {
        bool result = false;

        ContextType contextType = ContextType.Machine;

        if (InDomain())
        {
            contextType = ContextType.Domain;
        }

        try
        {
            using (PrincipalContext principalContext = new PrincipalContext(contextType))
            {
                result = principalContext.ValidateCredentials(
                    userName,
                    new NetworkCredential(string.Empty, securePassword).Password
                );
            }
        }
        catch (PrincipalOperationException)
        {
            // Account disabled? Considering as Login failed
            result = false;
        }
        catch (Exception)
        {
            throw;
        }

        return result;
    }

    /// <summary>
    ///     Validate: computer connected to domain?   
    /// </summary>
    /// <returns>
    ///     True -- computer is in domain
    ///     <para>False -- computer not in domain</para>
    /// </returns>
    public static bool InDomain()
    {
        bool result = true;

        try
        {
            Domain domain = Domain.GetComputerDomain();
        }
        catch (ActiveDirectoryObjectNotFoundException)
        {
            result = false;
        }

        return result;
    }

    public static System.Net.IPAddress? GetIP()
    {
        var host = System.Net.IPAddress.None;

        foreach (var ip in Dns.GetHostAddresses(Dns.GetHostName()))
        {
            host = ip;

            if (ip.AddressFamily == AddressFamily.InterNetwork) return host;
        }

        return null;
    }

    public bool SignIn(string login, string passwordHash)
    {
        var user = context.Users.FirstOrDefault(x => x.Login == login);
        
        if (user is null) return false;
        
        return user.PasswordHash == passwordHash;
    }
}
