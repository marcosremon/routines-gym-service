using Microsoft.AspNetCore.Mvc;
using RoutinesGymService.Application.DataTransferObject.Interchange.Auth.CheckTokenStatus;
using RoutinesGymService.Application.DataTransferObject.Interchange.Auth.Login;
using RoutinesGymService.Application.Interface.Application;
using RoutinesGymService.Transversal.Common.Responses;
using RoutinesGymService.Transversal.JsonInterchange.Auth.CheckTokenStatus;
using RoutinesGymService.Transversal.JsonInterchange.Auth.Login;
using RoutinesGymService.Transversal.JsonInterchange.Auth.LoginWeb;
using RoutinesGymService.Transversal.Security;

namespace RoutinesGymService.Service.WebApi.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthApplication _authApplication;

        public AuthController(IAuthApplication authApplication)
        {
            _authApplication = authApplication;
        }

        #region Login
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseJson>> Login([FromBody] LoginRequestJson loginRequestJson)
        {
            LoginResponseJson loginResponseJson = new LoginResponseJson();
            loginResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;

            try
            {
                if (loginRequestJson == null ||
                    string.IsNullOrEmpty(loginRequestJson.UserEmail) ||
                    string.IsNullOrEmpty(loginRequestJson.UserPassword))
                {
                    loginResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    loginResponseJson.IsSuccess = false;
                    loginResponseJson.Message = "Invalid data, the email or password is null or empty";
                }
                else
                {
                    LoginRequest loginRequest = new LoginRequest
                    {
                        UserEmail = loginRequestJson.UserEmail,
                        UserPassword = loginRequestJson.UserPassword
                    };

                    LoginResponse loginResponse = await _authApplication.Login(loginRequest);
                    if (loginResponse.IsSuccess)
                    {
                        loginResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        loginResponseJson.IsSuccess = loginResponse.IsSuccess;
                        loginResponseJson.Message = loginResponse.Message;
                        loginResponseJson.IsAdmin = loginResponse.IsAdmin;
                        loginResponseJson.BearerToken = loginResponse.IsAdmin 
                            ? JwtUtils.GenerateAdminJwtToken(loginRequest.UserEmail) 
                            : JwtUtils.GenerateUserJwtToken(loginRequest.UserEmail);
                    }
                    else
                    {
                        loginResponseJson.IsSuccess = loginResponse.IsSuccess;
                        loginResponseJson.Message = loginResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                loginResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                loginResponseJson.IsSuccess = false;
                loginResponseJson.Message = $"unexpected error on AuthController -> login: {ex.Message}";
            }

            return loginResponseJson;
        }
        #endregion

        #region Login web
        [HttpPost("login-web")]
        public async Task<ActionResult<LoginWebResponseJson>> LoginWeb([FromBody] LoginWebRequestJson loginWebRequestJson)
        {
            LoginWebResponseJson loginWebResponseJson = new LoginWebResponseJson();
            loginWebResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;

            try
            {
                if (loginWebRequestJson == null ||
                    string.IsNullOrEmpty(loginWebRequestJson.Email) ||
                    string.IsNullOrEmpty(loginWebRequestJson.Password))
                {
                    loginWebResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    loginWebResponseJson.IsSuccess = false;
                    loginWebResponseJson.Message = "Invalid data, the email or password is null or empty";
                }
                else
                {
                    LoginRequest loginWebRequest = new LoginRequest
                    {
                        UserEmail = loginWebRequestJson.Email,
                        UserPassword = loginWebRequestJson.Password
                    };

                    LoginResponse loginResponse = await _authApplication.Login(loginWebRequest);
                    if (loginResponse.IsSuccess)
                    {
                        loginWebResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                        loginWebResponseJson.IsSuccess = loginResponse.IsSuccess;
                        loginWebResponseJson.Message = loginResponse.Message;
                        loginWebResponseJson.BearerToken = loginResponse.IsAdmin
                            ? JwtUtils.GenerateAdminJwtToken(loginWebRequest.UserEmail)
                            : JwtUtils.GenerateUserJwtToken(loginWebRequest.UserEmail);
                    }
                    else
                    {
                        loginWebResponseJson.IsSuccess = loginResponse.IsSuccess;
                        loginWebResponseJson.Message = loginResponse.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                loginWebResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                loginWebResponseJson.IsSuccess = false;
                loginWebResponseJson.Message = $"unexpected error on AuthController -> login-web: {ex.Message}";
            }

            return Ok(loginWebResponseJson);
        }
        #endregion

        #region Check token status
        [HttpPost("check-token-status")]
        public ActionResult<CheckTokenStatusResponseJson> CheckTokenStatus([FromBody] CheckTokenStatusRequestJson checkTokenStatusRequestJson)
        {
            CheckTokenStatusResponseJson checkTokenStatusResponseJson = new CheckTokenStatusResponseJson();
            checkTokenStatusResponseJson.ResponseCodeJson = ResponseCodesJson.BAD_REQUEST;

            try
            {
                if (checkTokenStatusRequestJson == null ||
                    string.IsNullOrEmpty(checkTokenStatusRequestJson.Token))
                {
                    checkTokenStatusResponseJson.ResponseCodeJson = ResponseCodesJson.INVALID_DATA;
                    checkTokenStatusResponseJson.IsSuccess = false;
                    checkTokenStatusResponseJson.Message = "Invalid data, the token is null or empty";
                }
                else
                {
                    CheckTokenStatusRequest checkTokenStatusRequest = new CheckTokenStatusRequest
                    {
                        Token = checkTokenStatusRequestJson.Token
                    };
                    CheckTokenStatusResponse checkTokenStatusResponse = _authApplication.CheckTokenStatus(checkTokenStatusRequest);
                    if (checkTokenStatusResponse.IsValid)
                        checkTokenStatusResponseJson.ResponseCodeJson = ResponseCodesJson.OK;
                    else
                        checkTokenStatusResponseJson.ResponseCodeJson = ResponseCodesJson.UNAUTHORIZED;

                    checkTokenStatusResponseJson.IsValid = checkTokenStatusResponse.IsValid;
                    checkTokenStatusResponseJson.IsSuccess = checkTokenStatusResponse.IsSuccess;
                    checkTokenStatusResponseJson.Message = checkTokenStatusResponse.Message;
                }
            }
            catch (Exception ex)
            {
                checkTokenStatusResponseJson.ResponseCodeJson = ResponseCodesJson.INTERNAL_SERVER_ERROR;
                checkTokenStatusResponseJson.IsSuccess = false;
                checkTokenStatusResponseJson.Message = $"Unexpected error on AuthController -> check-token-status: {ex.Message}";
            }

            return Ok(checkTokenStatusResponseJson);
        }
        #endregion
    }
}