namespace Lab1.DAL.Entities
{
    public class PermissionGrantedOperation : Operation
    {
        public Guid GrantedUserId { get; set; }
        public User GrantedUser { get; set; }
    }
}
