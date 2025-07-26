﻿namespace RoutinesGymService.Transversal.Common
{
    public class BaseResponseJson
    {
        public ResponseCodesJson ResponseCodeJson { get; set; }
        public bool IsSuccess { get; set; } = false;
        public string? Message { get; set; }
    }
}