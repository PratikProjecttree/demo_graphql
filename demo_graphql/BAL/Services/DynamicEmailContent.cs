using Dapper;
using demo_graphql.Controllers;
using demo_graphql.Models.EmailModels;
using Newtonsoft.Json;
using System.Text;

namespace demo_graphql.Services
{
    public static class DynamicEmailContent
    {
        public static Dictionary<string, string> SetTemplate(string? content, PersonDetail member)
        {
            var data = content != null ? JsonConvert.DeserializeObject<List<DynamicVariables>>(content) : new List<DynamicVariables>();
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            if (data.Count > 0)
            {
                // var emailCodes = member?.QuestionAnswer?.Select(q => q.EmailCode).ToList() ?? new List<string>();
                foreach (var item in data)
                {
                    string? val = null;
                    switch (item.Variable)
                    {
                        case CommanDynamicParaNameForEmail.eventName:
                            val = member?.EventName ?? "";
                            break;
                        case CommanDynamicParaNameForEmail.ParticipantName:
                            val = member?.Name ?? "";
                            break;
                    }
                    dictionary.Add(item.Variable ?? "", val ?? "");
                }
            }
            return dictionary;
        }
        // public static Dictionary<string, string> SetTemplate(string? content, EmailQueueMemberResponse member)
        // {
        //     var data = content != null ? JsonConvert.DeserializeObject<List<DynamicVariables>>(content) : new List<DynamicVariables>();
        //     Dictionary<string, string> dictionary = new Dictionary<string, string>();
        //     if (data.Count > 0)
        //     {
        //         // var emailCodes = member?.QuestionAnswer?.Select(q => q.EmailCode).ToList() ?? new List<string>();
        //         foreach (var item in data)
        //         {
        //             string? val = null;
        //             switch (item.Variable)
        //             {
        //                 case CommanDynamicParaNameForEmail.eventName:
        //                     val = member?.EventName ?? "";
        //                     break;
        //                 case CommanDynamicParaNameForEmail.ParticipantName:
        //                     val = member?.FirstName + " " + member?.LastName ?? "";
        //                     break;
        //                 case CommanDynamicParaNameForEmail.eventStartDate:
        //                     val = member?.EventStartDate == null ? "" : item.Format != null ? Convert.ToDateTime(member?.EventStartDate).ToString(item.Format) : member?.EventStartDate.ToString();
        //                     break;
        //                 case CommanDynamicParaNameForEmail.eventStartDateF1:
        //                     val = member?.EventStartDate == null ? "" : item.Format != null ? Convert.ToDateTime(member?.EventStartDate).ToString(item.Format) : member?.EventStartDate.ToString();
        //                     break;
        //                 case CommanDynamicParaNameForEmail.eventEndDate:
        //                     val = member?.EventEndDate == null ? "" : item.Format != null ? Convert.ToDateTime(member?.EventEndDate).ToString(item.Format) : member?.EventEndDate.ToString();
        //                     break;
        //                 case CommanDynamicParaNameForEmail.eventEndDateF1:
        //                     val = member?.EventEndDate == null ? "" : item.Format != null ? Convert.ToDateTime(member?.EventEndDate).ToString(item.Format) : member?.EventEndDate.ToString();
        //                     break;
        //                 case CommanDynamicParaNameForEmail.registrationDate:
        //                     val = member?.RegistrationDate == null ? "" : item.Format != null ? Convert.ToDateTime(member?.RegistrationDate).ToString(item.Format) : member?.RegistrationDate.ToString();
        //                     break;
        //                 case CommanDynamicParaNameForEmail.registrationDateF1:
        //                     val = member?.RegistrationDate == null ? "" : item.Format != null ? Convert.ToDateTime(member?.RegistrationDate).ToString(item.Format) : member?.RegistrationDate.ToString();
        //                     break;
        //                 case CommanDynamicParaNameForEmail.paidAmount:
        //                     val = (member?.PaidAmount.ToString() ?? "0.00") + member?.CurrencyType ?? "USD";
        //                     break;
        //                 case CommanDynamicParaNameForEmail.registrationCloseDate:
        //                     val = member?.CloseParticipantRegistrationDate == null ? "" : item.Format != null ? Convert.ToDateTime(member?.CloseParticipantRegistrationDate).ToString(item.Format) : member?.CloseParticipantRegistrationDate.ToString();
        //                     break;
        //                 case CommanDynamicParaNameForEmail.RegistrationOpenDate:
        //                     val = member?.OpenParticipantRegistrationDate == null ? "" : item.Format != null ? Convert.ToDateTime(member?.OpenParticipantRegistrationDate).ToString(item.Format) : member?.OpenParticipantRegistrationDate.ToString();
        //                     break;
        //                 case CommanDynamicParaNameForEmail.registrationCloseDateF1:
        //                     val = member?.CloseParticipantRegistrationDate == null ? "" : item.Format != null ? Convert.ToDateTime(member?.CloseParticipantRegistrationDate).ToString(item.Format) : member?.CloseParticipantRegistrationDate.ToString();
        //                     break;
        //                 case CommanDynamicParaNameForEmail.transportationCloseDate:
        //                     val = member?.DueTransportationDate == null ? "" : item.Format != null ? Convert.ToDateTime(member?.DueTransportationDate).ToString(item.Format) : member?.DueTransportationDate.ToString();
        //                     break;
        //                 case CommanDynamicParaNameForEmail.transportationCloseDateF1:
        //                     val = member?.DueTransportationDate == null ? "" : item.Format != null ? Convert.ToDateTime(member?.DueTransportationDate).ToString(item.Format) : member?.DueTransportationDate.ToString();
        //                     break;
        //                 case CommanDynamicParaNameForEmail.eventAddress:
        //                     val = member?.EventAddress ?? "";
        //                     break;
        //                 case CommanDynamicParaNameForEmail.eventLocation:
        //                     val = member?.EventLocation ?? "";
        //                     break;
        //                 case CommanDynamicParaNameForEmail.accomodationCloseDate:
        //                     val = member?.DueAccomodationDate == null ? "" : item.Format != null ? Convert.ToDateTime(member?.DueAccomodationDate).ToString(item.Format) : member?.DueAccomodationDate.ToString();
        //                     break;
        //                 case CommanDynamicParaNameForEmail.accomodationCloseDateF1:
        //                     val = member?.DueAccomodationDate == null ? "" : item.Format != null ? Convert.ToDateTime(member?.DueAccomodationDate).ToString(item.Format) : member?.DueAccomodationDate.ToString();
        //                     break;
        //                 case CommanDynamicParaNameForEmail.outStandingAmount:
        //                     val = (member?.OutStandingAmount?.ToString() ?? "0.00") + member?.CurrencyType ?? "USD";
        //                     break;
        //                 case CommanDynamicParaNameForEmail.memberZone:
        //                     val = member?.MemberZone ?? "";
        //                     break;
        //                 case CommanDynamicParaNameForEmail.memberCenter:
        //                     val = member?.MemberCenter ?? "";
        //                     break;
        //                 case CommanDynamicParaNameForEmail.BAPSID:
        //                     val = member?.BAPSID ?? "";
        //                     break;
        //                 case CommanDynamicParaNameForEmail.misid:
        //                     val = member?.MisId.ToString() ?? "";
        //                     break;
        //                 case CommanDynamicParaNameForEmail.TransportationRequired:
        //                     val = val = member?.TransportationReqd != null ? member.TransportationReqd.ToString() : "";
        //                     break;
        //                 case CommanDynamicParaNameForEmail.ArrivalDate:
        //                     val = member?.ArrivalDate == null ? "" : item.Format != null ? Convert.ToDateTime(member?.ArrivalDate).ToString(item.Format) : member?.ArrivalDate.ToString();
        //                     break;
        //                 case CommanDynamicParaNameForEmail.ArrivalDateF1:
        //                     val = member?.ArrivalDate == null ? "" : item.Format != null ? Convert.ToDateTime(member?.ArrivalDate).ToString(item.Format) : member?.ArrivalDate.ToString();
        //                     break;
        //                 case CommanDynamicParaNameForEmail.ArrivalTime:
        //                     val = member?.ArrivalTime == null ? "" : DateTime.Today.Add((TimeSpan)member?.ArrivalTime).ToString("hh:mm tt");
        //                     break;
        //                 case CommanDynamicParaNameForEmail.ArrivalTimeF1:
        //                     val = member?.ArrivalTime == null ? "" : DateTime.Today.Add((TimeSpan)member?.ArrivalTime).ToString("HH:mm");
        //                     break;
        //                 case CommanDynamicParaNameForEmail.ArrivalAirportName:
        //                     val = member?.ArrivalAirport != null ? member.ArrivalAirport.ToString() : "";
        //                     break;
        //                 case CommanDynamicParaNameForEmail.ArrivalAirline:
        //                     val = member?.ArrivalAirline != null ? member.ArrivalAirline.ToString() : "";
        //                     break;
        //                 case CommanDynamicParaNameForEmail.ArrivalFlightNumber:
        //                     val = member?.ArrivalFlightNumber != null ? member.ArrivalFlightNumber.ToString() : "";
        //                     break;
        //                 case CommanDynamicParaNameForEmail.DepartureDate:
        //                     val = member?.DepartureDate == null ? "" : item.Format != null ? Convert.ToDateTime(member?.DepartureDate).ToString(item.Format) : member?.DepartureDate.ToString();
        //                     break;
        //                 case CommanDynamicParaNameForEmail.DepartureDateF1:
        //                     val = member?.DepartureDate == null ? "" : item.Format != null ? Convert.ToDateTime(member?.DepartureDate).ToString(item.Format) : member?.DepartureDate.ToString();
        //                     break;
        //                 case CommanDynamicParaNameForEmail.DepartureTime:
        //                     val = member?.DepartureTime == null ? "" : DateTime.Today.Add((TimeSpan)member?.DepartureTime).ToString("hh:mm tt");
        //                     break;
        //                 case CommanDynamicParaNameForEmail.DepartureTimeF1:
        //                     val = member?.DepartureTime == null ? "" : DateTime.Today.Add((TimeSpan)member?.DepartureTime).ToString("HH:mm");
        //                     break;
        //                 case CommanDynamicParaNameForEmail.DepartureAirportName:
        //                     val = member?.DepartureAirport != null ? member.DepartureAirport.ToString() : "";
        //                     break;
        //                 case CommanDynamicParaNameForEmail.DepartureAirline:
        //                     val = member?.DepartureAirline != null ? member?.DepartureAirline.ToString() : "";
        //                     break;
        //                 case CommanDynamicParaNameForEmail.DepartureFlightNumber:
        //                     val = member?.DepartureFlightNumber != null ? member?.DepartureFlightNumber.ToString() : "";
        //                     break;
        //                 case CommanDynamicParaNameForEmail.AccommodationType:
        //                     val = member?.AccommodationType != null ? member?.AccommodationType.ToString() : "";
        //                     break;
        //                 case CommanDynamicParaNameForEmail.DriverName:
        //                     val = member?.DriverName != null ? member?.DriverName.ToString() : "";
        //                     break;
        //                 case CommanDynamicParaNameForEmail.VehicleNumber:
        //                     val = member?.VehicleNumber != null ? member?.VehicleNumber.ToString() : "";
        //                     break;
        //                 case CommanDynamicParaNameForEmail.VehicleMakeName:
        //                     val = member?.VehicleMakeName != null ? member?.VehicleMakeName.ToString() : "";
        //                     break;
        //                 case CommanDynamicParaNameForEmail.VehicleModelName:
        //                     val = member?.VehicleModelName != null ? member?.VehicleModelName.ToString() : "";
        //                     break;
        //                 case CommanDynamicParaNameForEmail.VehicleColor:
        //                     val = member?.VehicleColor != null ? member?.VehicleColor.ToString() : "";
        //                     break;
        //                 case CommanDynamicParaNameForEmail.RequestType:
        //                     val = member?.RequestType != null ? member?.RequestType.ToString() : "";
        //                     break;
        //                 case CommanDynamicParaNameForEmail.TripStartDate:
        //                     val = member?.TripStartDate == null ? "" : item.Format != null ? Convert.ToDateTime(member?.TripStartDate).ToString(item.Format) : member?.TripStartDate.ToString();
        //                     break;
        //                 case CommanDynamicParaNameForEmail.TripNumber:
        //                     val = member?.TripNumber.ToString() ?? "";
        //                     break;
        //                 case CommanDynamicParaNameForEmail.MemberGroupName:
        //                     val = member?.MemberGroupName ?? "";
        //                     break;
        //                 case CommanDynamicParaNameForEmail.PinCode:
        //                     val = member?.Pin != null ? member?.Pin.ToString() : "";
        //                     break;
        //                 case CommanDynamicParaNameForEmail.sessionName:
        //                     val = member?.SessionName ?? "";
        //                     break;
        //                 case CommanDynamicParaNameForEmail.SpecialDietaryRestrictions:
        //                     val = string.IsNullOrEmpty(member?.OtherRestrictions) ? member?.DietaryRestrictions ?? "" : member?.DietaryRestrictions + ", (" + member?.OtherRestrictions + ')';
        //                     break;
        //             }
        //             dictionary.Add(item.Variable ?? "", val ?? "");
        //         }
        //     }
        //     return dictionary;
        // }
        // static string GenerateQrCodeBase64(string text)
        // {
        //     using (var qrGenerator = new QRCodeGenerator())
        //     {
        //         QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
        //         using (var qrCode = new PngByteQRCode(qrCodeData))
        //         {
        //             var bitmap = qrCode.GetGraphic(20);

        //             return Convert.ToBase64String(bitmap);
        //         }
        //     }
        // }
        // static string qrCodeEncode(string? bapsId, string? firstName, string? lastName, int? personId, string? email = null, int? memberId = null, int? eventId = null, string? guid = null)
        // {

        //     string qrCodeText;
        //     if (!string.IsNullOrEmpty(email) && memberId != null && string.IsNullOrEmpty(bapsId) && string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName) && personId == null)
        //     {
        //         qrCodeText = string.Concat("E:", email, "|MemberId:", memberId, "|EventId:", eventId, "|Guid:", guid);
        //     }
        //     else if (string.IsNullOrEmpty(bapsId))
        //     {
        //         qrCodeText = string.Concat("M:", personId, "|F:", firstName, "|L:", lastName);
        //     }
        //     else
        //     {
        //         qrCodeText = string.Concat("B:", bapsId, "|F:", firstName, "|L:", lastName);
        //     }

        //     var plainTextBytes = Convert.ToBase64String(Encoding.UTF8.GetBytes(qrCodeText));
        //     plainTextBytes = plainTextBytes?.Replace("=", "");
        //     var last23char = plainTextBytes?.Substring(plainTextBytes.Length - 23);
        //     var without23char = plainTextBytes?.Substring(0, plainTextBytes.Length - 23);
        //     var qrCode = string.Concat(last23char, without23char);
        //     var sqrCode = string.Concat("data:image/png;base64,", GenerateQrCodeBase64(qrCode));
        //     return sqrCode;
        // }
        static string tripIdEncode(int? tripId, DateTime? tripEndDate)
        {
            string tripIdText;
            tripIdText = string.Concat("T:", tripId);
            var plainTextBytes = Convert.ToBase64String(Encoding.UTF8.GetBytes(tripIdText));

            // Remove '=' padding
            plainTextBytes = plainTextBytes.Replace("=", "");

            // If shorter than 23 chars, just return the string as-is
            if (plainTextBytes.Length <= 23)
                return plainTextBytes;

            // Rearrange string (last 23 characters moved to front)
            var last23char = plainTextBytes.Substring(plainTextBytes.Length - 23);
            var without23char = plainTextBytes.Substring(0, plainTextBytes.Length - 23);

            var tripData = string.Concat(last23char, without23char);
            return tripData;
        }
        static string RsvpMemberEventEncode(int eventId, int rsvpMemberId)
        {
            string rsvpMemberEventText;
            rsvpMemberEventText = string.Concat(eventId, "-", rsvpMemberId);
            var plainTextBytes = Convert.ToBase64String(Encoding.UTF8.GetBytes(rsvpMemberEventText));
            return plainTextBytes;
        }
        public static string SetTemplateContent(EmailQueueSchedularResponse? model, EmailQueueMemberResponse member)
        {
            var content = model?.Body;
            var data = model?.DynamicVariables != null ? JsonConvert.DeserializeObject<List<DynamicVariables>>(model.DynamicVariables) : new List<DynamicVariables>();
            if (data.Count > 0)
            {
                foreach (var item in data)
                {
                    string? val = null;
                    switch (item.Variable)
                    {
                        case CommanDynamicParaNameForEmail.eventName:
                            val = member?.EventName ?? "";
                            break;
                        case CommanDynamicParaNameForEmail.ParticipantName:
                            val = member?.FirstName + " " + member?.LastName ?? "";
                            break;
                        case CommanDynamicParaNameForEmail.eventStartDate:
                            val = member?.EventStartDate == null ? "" : item.Format != null ? Convert.ToDateTime(member?.EventStartDate).ToString(item.Format) : member?.EventStartDate.ToString();
                            break;
                        case CommanDynamicParaNameForEmail.eventStartDateF1:
                            val = member?.EventStartDate == null ? "" : item.Format != null ? Convert.ToDateTime(member?.EventStartDate).ToString(item.Format) : member?.EventStartDate.ToString();
                            break;
                        case CommanDynamicParaNameForEmail.eventEndDate:
                            val = member?.EventEndDate == null ? "" : item.Format != null ? Convert.ToDateTime(member?.EventEndDate).ToString(item.Format) : member?.EventEndDate.ToString();
                            break;
                        case CommanDynamicParaNameForEmail.eventEndDateF1:
                            val = member?.EventEndDate == null ? "" : item.Format != null ? Convert.ToDateTime(member?.EventEndDate).ToString(item.Format) : member?.EventEndDate.ToString();
                            break;
                        case CommanDynamicParaNameForEmail.registrationDate:
                            val = member?.RegistrationDate == null ? "" : item.Format != null ? Convert.ToDateTime(member?.RegistrationDate).ToString(item.Format) : member?.RegistrationDate.ToString();
                            break;
                        case CommanDynamicParaNameForEmail.registrationDateF1:
                            val = member?.RegistrationDate == null ? "" : item.Format != null ? Convert.ToDateTime(member?.RegistrationDate).ToString(item.Format) : member?.RegistrationDate.ToString();
                            break;
                        case CommanDynamicParaNameForEmail.paidAmount:
                            val = (member?.PaidAmount.ToString() ?? "0.00") + member?.CurrencyType ?? "USD";
                            break;
                        case CommanDynamicParaNameForEmail.registrationCloseDate:
                            val = member?.CloseParticipantRegistrationDate == null ? "" : item.Format != null ? Convert.ToDateTime(member?.CloseParticipantRegistrationDate).ToString(item.Format) : member?.CloseParticipantRegistrationDate.ToString();
                            break;
                        case CommanDynamicParaNameForEmail.registrationCloseDateF1:
                            val = member?.CloseParticipantRegistrationDate == null ? "" : item.Format != null ? Convert.ToDateTime(member?.CloseParticipantRegistrationDate).ToString(item.Format) : member?.CloseParticipantRegistrationDate.ToString();
                            break;
                        case CommanDynamicParaNameForEmail.transportationCloseDate:
                            val = member?.DueTransportationDate == null ? "" : item.Format != null ? Convert.ToDateTime(member?.DueTransportationDate).ToString(item.Format) : member?.DueTransportationDate.ToString();
                            break;
                        case CommanDynamicParaNameForEmail.transportationCloseDateF1:
                            val = member?.DueTransportationDate == null ? "" : item.Format != null ? Convert.ToDateTime(member?.DueTransportationDate).ToString(item.Format) : member?.DueTransportationDate.ToString();
                            break;
                        case CommanDynamicParaNameForEmail.eventAddress:
                            val = member?.EventAddress ?? "";
                            break;
                        case CommanDynamicParaNameForEmail.eventLocation:
                            val = member?.EventLocation ?? "";
                            break;
                        case CommanDynamicParaNameForEmail.accomodationCloseDate:
                            val = member?.DueAccomodationDate == null ? "" : item.Format != null ? Convert.ToDateTime(member?.DueAccomodationDate).ToString(item.Format) : member?.DueAccomodationDate.ToString();
                            break;
                        case CommanDynamicParaNameForEmail.accomodationCloseDateF1:
                            val = member?.DueAccomodationDate == null ? "" : item.Format != null ? Convert.ToDateTime(member?.DueAccomodationDate).ToString(item.Format) : member?.DueAccomodationDate.ToString();
                            break;
                        case CommanDynamicParaNameForEmail.outStandingAmount:
                            val = (member?.OutStandingAmount?.ToString() ?? "0.00") + member?.CurrencyType ?? "USD";
                            break;

                    }
                    content = content?.Replace("${" + item.Variable + "}", val);
                }
            }
            return content;
        }
        // public static byte[] qrCodeEncodeWithBytes(string? bapsId, string? firstName, string? lastName, int? personId, string? email = null, int? memberId = null, int? eventId = null, string? guid = null)
        // {
        //     string qrCodeText;

        //     if (!string.IsNullOrEmpty(email) && memberId != null &&
        //         string.IsNullOrEmpty(bapsId) &&
        //         string.IsNullOrEmpty(firstName) &&
        //         string.IsNullOrEmpty(lastName) &&
        //         personId == null)
        //     {
        //         qrCodeText = $"E:{email}|MemberId:{memberId}|EventId:{eventId}|Guid:{guid}";
        //     }
        //     else if (string.IsNullOrEmpty(bapsId))
        //     {
        //         qrCodeText = $"M:{personId}|F:{firstName}|L:{lastName}";
        //     }
        //     else
        //     {
        //         qrCodeText = $"B:{bapsId}|F:{firstName}|L:{lastName}";
        //     }

        //     var plainTextBytes = Convert.ToBase64String(Encoding.UTF8.GetBytes(qrCodeText));
        //     plainTextBytes = plainTextBytes?.Replace("=", "");
        //     var last23char = plainTextBytes?.Substring(plainTextBytes.Length - 23);
        //     var without23char = plainTextBytes?.Substring(0, plainTextBytes.Length - 23);
        //     var qrCode = string.Concat(last23char, without23char);

        //     using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
        //     using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrCode, QRCodeGenerator.ECCLevel.Q))
        //     using (var qrCodePng = new PngByteQRCode(qrCodeData))
        //     {
        //         return qrCodePng.GetGraphic(20); // returns byte[]
        //     }
        // }



    }
}