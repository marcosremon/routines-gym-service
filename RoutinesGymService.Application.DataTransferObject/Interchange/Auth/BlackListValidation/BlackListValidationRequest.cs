namespace RoutinesGymService.Application.DataTransferObject.Interchange.Auth.BlackListValidation
{
    public class BlackListValidationRequest
    {
        public string MobileGuid { get; set; } = string.Empty;
        public string UserSerialNumber { get; set; } = string.Empty;
        public string UserPassword { get; set; } = string.Empty;
        public byte[] EncryptedPassword { get; set; } = Array.Empty<byte>();
    }
}