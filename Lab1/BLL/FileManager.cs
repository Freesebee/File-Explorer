using Lab1.DAL;
using Lab1.DAL.Entities;
using Lab1.Models;
using Microsoft.EntityFrameworkCore;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Net;
using System.Security;
using System.Security.Principal;
using IPAddress = Lab1.DAL.Entities.IPAddress;

namespace Lab1.BLL;

public class FileManager
{
    private readonly AppDbContext context;

    public FileManager(AppDbContext context)
    {
        this.context = context;
    }

    public bool IsHostRegistered()
    {
        var current = WindowsIdentity.GetCurrent();
        return context.Users.Any(x => x.Login == current.Name);
    }

    public User RegisterUser(UserRegistraionModel registraionModel)
    {
        var user = new User()
        {
            IPAddresses = new List<IPAddress>()
                {
                    new() { Address = registraionModel.IPAddress }
                },
            Login = registraionModel.Login,
            Password = registraionModel.Password,
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

    public void AddUserIp(Guid userId, string ipAddress)
    {
        var user = context.Users.First(x => x.Id == userId);

        var ipEntity = new IPAddress()
        {
            Address = ipAddress,
        };

        user.IPAddresses.Add(ipEntity);

        context.SaveChanges();
    }

    public void RemoveUserIp(Guid userId, Guid ipId)
    {
            var user = context.Users.First(x => x.Id == userId);
            var ipEntity = context.IPAddresses.First(x => x.Id == ipId);

            user.IPAddresses.Remove(ipEntity);
            context.IPAddresses.Remove(ipEntity);

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
}
