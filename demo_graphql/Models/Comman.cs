
namespace demo_graphql.Controllers
{
    public static class Category
    {
        public const string Default = "default";
        public const string Workflow = "workflow";
        public const string Custom = "custom";
    }
    public static class QueryType
    {
        public const string Query = "Query";
        public const string Mutation = "Mutation";
    }

    public static class AllowedUpdateOperators
    {
        public static readonly List<string> AllowedOperators = new List<string> { "_eq", "_in", "_and" };
    }
    public static class SystemModuleCode
    {
        public const string PaymentConfiguration = "PaymentConfiguration";
        public const string MasterEvent = "MasterEvent";
    }
    public static class ModuleAction
    {
        public const string View = "View";
        public const string Add = "Add";
        public const string Edit = "Edit";
        public const string Delete = "Delete";
    }
    public static class OperationType
    {
        public const string Insert = "insert_";
        public const string Update = "update_";
        public const string Query = "Query";
    }
    public static class CommonMessage
    {
        public const string APP_CONFIG_MISSING = "AppSetting Cofiguration Missing.";
        public const string BAD_DATA = "Model Validation Failed.";
        public const string NO_DATA_FOUND = "Requested data was not found.";
        public const string RECORD_ALREADY_EXISTS = "Record already exists.";
        public const string SUCCESS = "Success";
        public const string INTERNAL_SERVER_ERROR = "Internal server error";
        public const string DATA_ADD_SUCCESS = "Data has been added successfully.";
        public const string DATA_UPDATE_SUCCESS = "Data has been updated successfully.";
        public const string DATA_DELETE_SUCCESS = "Data has been deleted successfully.";
        public const string ADD_REQUIRED_INFORMATION = "Please add required information.";
        public const string MEMBER_NOT_EXIST = "Member data not found.";
        public const string EVENT_NOT_EXIST = "Event data not found.";
        public const string SELECT_ATLEAST_ONE_SPONSERSHIP = "Select atleast one sponsership.";
        public const string NO_PAYABLE_AMOUNT = "There is no payble amount.";
        public const string OVER_SLOT_LIMIT = "registration limit is over";
        public const string SELECT_ATLEAST_ONE_PARAMETER = "Select atleast one search criteria.";
        public const string MAILTEMPLATE_ALREADY_EXIST = "Email Templates with this Group and Email Type already exist.";
        public const string ADDRESS_REQUIRED = "Address informations is required.";
        public const string EXCEED_ALLOCATION_LIMIT = "allocation exceed so you can not change capacity";
        public const string PERMISSIOM_DENIED_CHANGE_DATA = "You don't have access to change data.";
        public const string NO_DATA_ADDED = "Requested clone record added.";
        public const string KUND_ALLOCATED = "Kund already allocated. Please remove kund selection if you want update";
        public const string INVALID_MIS = "Invalid MISID, Email, or Phone Number. Please check your input and try again.";
        public const string DUPLICATE_DATA = "We found duplicate records. Please check input data.";
        public const string WRONG_FILE_FORMAT = "Wrong File Format";
        public const string INVALID_DATA = "Invalid Data";
        public const string EVENT_LOCK = "Registration for the event has ended.";
        public const string YAGNA_EXIST = "Yagna exist on this member.";
        public const string YAGNA_SLOT_INACTIVE = "Can not change as original slot is inactive.";
        public const string INVALID_AMOUNT = "selected amount is not valid.";
        public const string ALREADY_EXISTS_GROUPANDTITLE = "Field title already exists.";
        public const string RECORD_NOT_FOUND = "Record not found";
        public const string RECORD_IN_USE = "This record already in use.";
        public const string ACCOMODATION_ALREADY_ASSIGN = "Your accommodation is assigned, You can't change your accommodation details.";
        public const string ASSIGNMENT_CAPACITY_IS_OVER = "Capacity is over.";
        public const string ACCOMMODATION_NOT_AVAILABLE = "Accommodation not available in this date range.";
        public const string ROOM_NOT_AVAILABLE = "Room not available in this date range.";
        public const string DATE_RANGE = "Seleted Date is not in range.";
        public const string ROOM_ALREADY_EXISTS = "Room number already exists.";
        public const string HOTEL_ROOM_ALREADY_EXISTS = "Hotel room number already exists.";
        public const string CAPACITY_NOT_DECREASE = "Decrease in capacity not allowed; room is assigned";
        public const string BED_ALREADY_EXISTS = "Bed number already exists.";
        public const string REGISTRATION_DETAIL_NOT_FOUND = "Registration detail not found.";
        public const string REGISTRATION_ROOM_DETAIL_NOT_FOUND = "Registration room detail not found.";
        public const string ACCOMMODATION_REMOVE_SUCCESS = "Accommodation removed successfully.";
        public const string ACCOMMODATION_DETAIL_NOT_FOUND = "Accommodation details are not found.";
        public const string ALREADY_ONE_ENABLE_CONFIGURATION = "Only Add one Enable payment Configuration.";
        public const string ALREADY_EXISTS_NAME = "Already exist name in this Event.";
        public const string ALREADY_EXISTS_ANNOUNCEMENT_TITLE = "Announcement title already exists.";
        public const string ALREADY_EXISTS_EVENTSUMMARY_TITLE = "Event summary title already exists.";
        public const string ALREADY_EXISTS_SECTION_TITLE = "Section title already exists.";
        public const string ALREADY_EXISTS_ACCOMADATION_TYPE_NAME = "Already Exist Name In Accomdation Type.";
        public const string ALREADY_EXISTS_TAG = "Tag already exist.";
        public const string ALREADY_EXISTS_EVENT_TAB = "Event tab already exist.";
        public const string ALREADY_EXISTS_EVENTGROUP = "EventGroup already exist.";
        public const string ALREADY_EXISTS_SURVEY = "Survey already exist.";
        public const string ALREADY_EXISTS_SLIDER = "Slider already exist.";
        public const string ALREADY_EXISTS_ROLE = "Role already exist.";
        public const string ALREADY_EXISTS_Goshthi = "Goshthi already exist.";
        public const string PAYMENT_CONFIG_MISSING = "We couldn’t process you payment .";
        public const string EXCEED_LIMIT = "Maximum limit has reached";
        public const string PERMISSION_DENIED_ENTITY = "selected data's entity has no permission in this Event";
        public const string CHECK_IN_NAME_ALREADY_EXISTS = "Checkin name already exists.";
        public const string CHECK_IN_EVENTGROUP_NOT_FOUND = "Selected eventgroup not found.";
        public const string CHECK_IN_EVENT_NOT_FOUND = "Selected event not found";
        public const string CHECK_IN_TYPE_NOT_FOUND = "Selected checkin type not found";
        public const string CATEGORY_ALREADY_EXISTS = "Checklist category already exists in another checkin";
        public const string CHECK_IN_RECORD_NOT_FOUND = "Checkin record not found";
        public const string CHECK_IN_PERSON_NOT_FOUND = "Person not found";
        public const string CHECK_IN_PERSON_OR_MEMBER_NOT_FOUND = "Person or member not found";
        public const string ALREADY_INCLUDE_DEFAULT_SURVEY = "Already included default survey.";
        public const string SURVEY_ALREADY_EXISTS = "Survey already exists";
        public const string PERMISSIOM_DENIED_ROLE = "You don't have access to add Event Admin or Same role.";
        public const string MEMBER_REGISTERED_NOT_DELETE = "You can not delete registered Member.";
        public const string PERMISSIOM_DENIED_ACCESS_EVENT = "You don't have Permission to access this Event.";
        public const string PARTICIPANT_NOT_ELIGIBLE_TO_REGISTER_IN_EVENT = "You are not eligible for this event.";
        public const string PARTICIPANT_NOT_ELIGIBLE_IN_EVENT = "Participant is not eligible for this event.";
        public const string EXCEPTION_OCCURED = "Some kind of error occurred in the API.  Please use the id and contact our " +
                        "support team if the problem persists.";
        public const string ROLE_ALREADY_ASSIGN = "Role already assigned to another user so do not delete this role.";
        public const string GROUP_ALREADY_EXISTS = "Group name already exists.";
        public const string REGISTER_PARTICIPANT_EXISTS = "You do not have remaining registered participant to add this group";
        public const string MEMBER_ALREADY_EXISTS_IN_EVENT = "Member already exists in the selected event.";
        public const string REGISTERATION_DATE_CLOSED = "you do not have permission to register after registration date is closed.";
        public const string PARTICIPANT_REGISTRATION_CAPACITY_OVER = "Participant registration capacity is over.";
        public const string INVALID_GENDER = "Invalid Gender.";
        public const string ALREADY_EXISTS_IDENTITY_CARD = "Identity card already exists.";
        public const string NO_ACCESS_REGISTRATION = "You do not have access to make any changes";
        public const string CHECK_IN_ATTENDANCE_ALREADY_DONE = "{0} already saved.";
        public const string CHECK_IN_ATTENDANCE_SUCCESS = "{0} saved successfully.";
        public const string MAXIMUM_LIMIT_REACHED = "Maximum limit reached.";
        public const string NO_PERMISSION_TO_ATTEND_EVENT = "You are not attending this event.";
        public const string MEMBER_REGISTERED_BY_SUBEVENT = "Member already registered by subevent.";
        public const string MEMBER_ALREADY_REGISTERED = "Member already registered you can not change";
        public const string USER_ONE_ROLE_ASSIGN = "User should have minimum one role required";
        public const string USER_NOT_FOUND = "User not found";
        public const string LOGIN_SUCCESS = "Logged in successfully";
        public const string ACCESS_DENIED = "Access is denied";
        public const string SELECT_RESTRICTION_SLOT = "Please select a restriction slots.";
        public const string SELECT_SLOT_NOT_INCLUDE_MAILSEND = "Selected slot is not include in mailSend.";
        public const string SEARCH_ATLEAST_ONE_FILTER = "Please search by atleast one filter.";
        public const string SEARCH_FIRSTNAME_LASTNAME_WITH_ID = "please search by firstname and lastname with Id.";
        public const string SEARCH_WITH_OTHER_FILTER_FIRSTNAME_LASTNAME = "please search with other filter with firstname or lastname";
        public const string MEMBER_ALREADY_EXISTS_ANOTHER_EVENT = "This member already exists in this event";
        public const string NO_DATA_FOUND_EMAILTEMPLATE = "No Data Found of EmailTemplate.";
        public const string DAYCARE_INVALID = "Daycare Type Is Invalid.";
        public const string SUBEVENT_REQUIRED = "SubEvent is Required.";
        public const string CATEGORY_NOT_MATCHED = "CategoryId does not matched.";
        public const string MANDALID_NOT_MATCHED = "MandalId does not matched.";
        public const string MODULE_ACTION_ID_NOT_MATCHED = "ModuleActionId does not matched.";
        public const string MEMBER_NOT_INVITE = "Member is not invited.";
        public const string ALREADY_ADDED_DISCOUNT = "Already added discount in {0} Member.";
        public const string PAYMENT_CONFIG_NOT_FOUND = "You can not proceed refund as config is missing.";
        public const string INVALID_STATUS = "selectes status is not valid.";
        public const string PAYMENT_NOT_FOUND = "Payment is not found for refund.";
        public const string REFUND_CANCELLED = "Refund has been failed.";
        public const string EARLY_BIRD_NOT_GRATER_REGISTRATION_AMOUNT = "Early bird amount cannot be greater than registration Amount";
        public const string DO_NOT_DECREASE_ROOM = "You cannot decrease total number of rooms";
        public const string ROOM_ALREADY_ASSIGN = "Room is already assigned; it cannot be deleted";
        public const string ROOM_ASSIGNED_CAPACITY_NOT_DECREASE = "Room is assigned; capacity cannot be decreased";
        public const string TRANSPORATION_DUE_DATE_FINISH_CANT_CHANGE = "Transporation cannot be updated after due date is over";
        public const string DAYCARE_DUE_DATE_FINISH_CANT_CHANGE = "Daycare cannot be updated after due date is over";
        public const string ACCOMMODATION_INVENTORY_AND_MEMBER__GENDER_NOT_MATCHED = "Inventory and participant gender does not matched";
        public const string SUBEVENT_NOT_MATCH = "Session does not matched";
        public const string PLIST_KARYAKAR_NOT_MATCED = "You cannot invited karyakar via selected plist";
        public const string QUESTION_DUEDATE_GREATER_REGISTRATION_CLOSE_DATE = "Due date should be less and equal than registration closing date.";
        public const string FIELD_NOT_UPDATE_AFTER_DUEDATE = "Field cannot be updated after the due date.";
        public const string PARTICIPANT_ALREADY_REGISTERED = "You are already registered.";
        public const string QUESTION_OPTION_ALREADY_EXISTS = "Source option already exists.";
        public const string HIERARACY_NOT_ALLOWED = "Hierarchical source field should not be added in target fields.";
        public const string PHONE_NUMBER_EVENT_BASE_RESTRICTION = "No participant found for this event; unable to retrieve phone number.";
        public const string EMAILINFO_EVENT_BASE_RESTRICTION = "No participant found for this event; unable to retrieve Email.";
        public const string RSVP_EMAIL_NOT_FOUND_IN_MIS = "Participant email is not found.";
        public const string INVALID_QRCODE = "Invalid QrCode.";
        public const string EMAILCODE_ALREADY_EXISTS = "Email code is already used in another fields.";
        public const string QUESTION_OPTION_ONE_DEFAULT = "Only one option can be marked as default.";
        public const string EVENT_ACCESS_DENIED = "You havent't access data for this event.";
        public const string PARTICIPANT_NOT_FOUND = "Participant not found.";
        public const string ATLEAST_ONE_QUESTION_REQUIRED = "Minimum one field is required for Table type field";
        public const string SECTION_ALREADY_USE = "Section already in use";
        public const string PARTIAL_AMOUNT_LESS_THAN_REAMAINING_AMOUNT = "Partial refund amount must be less than remaining paid amount.";
        public const string REQUESTED_PAIR_NOT_FOUND = "We couldn’t found any matched data. Please try again.";
        public const string MISID_IS_REQUIRED = "MISID must be provided along with an email or phone number.";
        public const string EMAIL_OR_PHONENUMBER_REQUIRED = "MISID must be provided along with an email or phone number.";
        public const string MISID_ALLOW_NUMBER = "MISID should be Numbers only.";
        public const string FILE_UPLOAD_ERROR = "File upload url request failed due to an unexpected error.";
        public const string SFS_CONFIGURATION_REQUIRED = "SFS Service configuration required.";
        public const string ACCOMMODATION_YES_THAN_ALL_PARTICIPANTS_REQUIRED_TO_USE_REQUIRED = "Accommodation should be opted yes for selecting 'Are all participants required to use sanstha-managed accommodations?' question's option.";
        public const string ACCOMMODATION_REQUIRED = "Accommodation must be selected.";
        public const string EVENT_HAS_BEEN_CLOSED = "Event has been closed.";
        public const string EVENT_REGISTRATION_HAS_BEEN_CLOSED = "Registration has been closed.";
        public const string SIGN_ALREADY_EXISTS = "Invalid request; SignImage already exists.";
        public const string SIGNIMAGE_REQUIRED = "Invalid request; SignImage is required.";
        public const string UNEXPECTED_DAYCARE_INFORMATION = "Invalid request; Unexpected daycare information found.";
        public const string UNEXPECTED_DIETARY_RESTRICTION_INFORMATION = "Invalid request; Unexpected dietary restriction information found.";
        public const string MODEL_VALIDATION_FAILED_DUE_TO_LIABILITY_INFORMATION = "Model validation failed due to liablity information.";
        public const string POSITION_NOT_FOUND = "Entered data is not linked to Karyakar.";
        public const string DUBLICATE_SEVAROLE_FOUND = "Duplicate SevaRole found.";
        public const string ROLE_DEPARTMENT_ALREADY_EXISTS = "Selected ERS role and seva department already exists.";
        public const string UPLOAD_RECORDS_COUNT_SHOULD_NOT_EXCEED_500 = "The number of records you upload should not exceed 500.";
        public const string TAG_NOT_EXIST = "Tag not found!";
        public const string TAG_ASSIGNED = "Tag(s) successfully assigned to participants.";
        public const string TAG_UNASSIGNED = "Tag(s) successfully unassigned from participants.";
        public const string DRIVER_ALREADY_EXISTS = "Driver is already exists.";
        public const string VEHICLE_ALREADY_EXISTS = "Vehicle is already exists.";
        public const string DRIVER_VEHICLE_UNASSIGN = "Driver Vehicle unassigned successfully.";
        public const string DRIVER_VEHICLE_ASSIGN = "Driver Vehicle assigned successfully.";
        public const string DRIVER_NOT_FOUND = "Driver does not exist";
        public const string DRIVER_ALREADY_FOUND = "Driver vehicle already assigned.";
        public const string VEHICLE_NOT_FOUND = "Vehicle does not exist";
        public const string VEHICLETYPE_NOT_VALID = "Vehicle Type not valid.";
        public const string VEHICLEMAKE_NOT_VALID = "Vehicle Make not valid.";
        public const string ISSUEAUTHROITY_NOT_VALID = "IssueAuthority not valid.";
        public const string DRIVERLICENSETYPE_NOT_VALID = "Driver License Type not valid.";
        public const string DRIVER_ASSIGN_IN_TRNSPORATION = "Driver cannot be deleted while assigned to transportation.";
        public const string TAG_ALREADY_ASSIGNED = "Tag already in use.";
        public const string VEHICLE_ASSIGN_IN_TRNSPORATION = "Vehicle cannot be deleted while assigned to transportation.";
        public const string TEMPLATE_ALREADY_EXIST = "Form template already exist.";
        public const string TRIP_NOT_FOUND = "Trip not found.";
        public const string TRIP_DRIVER_UNASSIGNED = "Trip Unassigned successfully.";
        public const string TEMPLATE_APPLIED_SUCCESSFULLY = "Template applied successfully.";
        public const string TEMPLATE_ALREADY_APPLIED = "Template already applied to this section.";
        public const string TEMPLATE_LIMIT_EXCEED = "Cannot apply template - exceeds 50 fields limit per section.";
        public const string TEMPLATE_NO_ACTIVE_FIELD = "Template has no active fields.";
        public const string INVALID_SECTION = "Invalid section.";
        public const string INVALID_TEMPLATE = "Invalid template.";
        public const string EVENT_NOT_RSVP = "Rsvp is not enabled for this event.";
        public const string EVENT_CONFIG_MISMATCH = "RSVP MIS ID setting mismatch.";
        public const string BAPS_MIS_NOT_FOUND = "BAPS/MISID not found.";
        public const string FIRST_NAME_OR_LAST_NAME_EMAIL_IS_REQUIRED = "First name or last name or email is required.";
        public const string PARTICIPANT_ALREADY_EXISTS = "Participant Already Exists.";
        public const string INVALID_FIRST_NAME = "First name must contain only letters.";
        public const string INVALID_LAST_NAME = "Last name must contain only letters.";
        public const string INVALID_PHONE_CHARACTERS = "Phone number can only contain digits, '+', '-' and spaces.";
        public const string INVALID_PHONE_LENGTH = "Phone number must contain between 10 to 15 digits.";
        public const string INVALID_EMAIL_FORMAT = "Email format is invalid.";
        public const string MAX_RECORD_LIMIT = "You can upload a maximum of 4000 records at a time. You uploaded";
        public const string INVALID_PIN = "Invalid PIN.";
        public const string RSVP_MEMBER_ID_NOT_FOUND = "RSVP member id not found.";
        public const string INVALID_ACTION_TYPE = "Invalid action type.";
        public const string OPPOSITE_RULE_EXISTS = "This field already has an opposite rule set.";
        public const string FIELD_NOT_EXISTS = "No fields yet set for this event.";
        public const string FIELD_NOT_ALLOW_DELETE = "Cannot delete the only field when members are invited or registered.";
        public const string INPUT_DATA_INVALID = "Input data is invalid.";
        public const string EVENT_CREATION_NOT_ALLOWED = "You don’t have permission to create an event under this entity.";
        public const string USER_NOT_ALLOWED = "User cannot be added as they are from a different division.";
        public const string ENTITY_NOT_BELONG_TO_DIVISION = "Selected entity does not belong to your division.";
        public const string PERSON_NOT_ALLOWED = "Person cannot be added as they are from another division.";
        public const string DEPARTMENT_DIVISION_MISMATCH = "One or more selected departments do not belong to this division.";
        public const string COUNTRY_DIVISION_MISMATCH = "Selected country does not belong to this division.";
        public const string STATE_COUNTRY_MISMATCH = "Selected state is not valid for the selected country.";
        public const string EVENT_GROUP_DIVISION_MISMATCH = "Selected event group does not belong to this division.";
        public const string AIRPORT_DIVISION_MISMATCH = "One or more selected airports do not belong to this division.";
        public const string PAYMENT_CONFIG_DIVISION_MISMATCH = "Selected payment configuration does not belong to this division.";
        public const string AIRLINE_IN_USE = "Airline details already in use, so cannot be deleted.";
    }
    public static class CommanDynamicParaNameForEmail
    {
        public const string eventName = "eventName";
        public const string eventStartDate = "eventStartDate";
        public const string eventEndDate = "eventEndDate";
        public const string eventStartDateF1 = "eventStartDateF1";
        public const string eventEndDateF1 = "eventEndDateF1";
        public const string ParticipantName = "ParticipantName";
        public const string paidAmount = "PaidAmount";
        public const string registrationDate = "registrationDate";
        public const string registrationCloseDate = "registrationCloseDate";
        public const string registrationDateF1 = "registrationDateF1";
        public const string registrationCloseDateF1 = "registrationCloseDateF1";
        public const string transportationCloseDate = "transportationCloseDate";
        public const string accomodationCloseDate = "accomodationCloseDate";
        public const string transportationCloseDateF1 = "transportationCloseDateF1";
        public const string accomodationCloseDateF1 = "accomodationCloseDateF1";
        public const string eventLocation = "eventLocation";
        public const string eventAddress = "eventAddress";
        public const string outStandingAmount = "outStandingAmount";
        public const string registeredFamilyMemberList = "registeredFamilyMemberList";
        public const string additionalGuestList = "additionalGuestList";
        public const string memberZone = "memberZone";
        public const string memberCenter = "memberCenter";
        public const string BAPSID = "BAPSID";
        public const string misid = "misid";
        public const string qrCode = "qrCode";
        public const string sessionName = "sessionName";
        public const string SpecialDietaryRestrictions = "specialDietaryRestrictions";
        public const string TransportationRequired = "TransportationRequired";
        public const string ArrivalDate = "ArrivalDate";
        public const string ArrivalDateF1 = "ArrivalDateF1";
        public const string ArrivalTime = "ArrivalTime";
        public const string ArrivalTimeF1 = "ArrivalTimeF1";
        public const string ArrivalAirportName = "ArrivalAirportName";
        public const string ArrivalAirline = "ArrivalAirline";
        public const string ArrivalFlightNumber = "ArrivalFlightNumber";
        public const string DepartureDate = "DepartureDate";
        public const string DepartureDateF1 = "DepartureDateF1";
        public const string DepartureTime = "DepartureTime";
        public const string DepartureTimeF1 = "DepartureTimeF1";
        public const string DepartureAirportName = "DepartureAirportName";
        public const string DepartureAirline = "DepartureAirline";
        public const string DepartureFlightNumber = "DepartureFlightNumber";
        public const string AccommodationType = "AccommodationType";
        public const string DriverName = "driverName";
        public const string VehicleNumber = "vehicleNumber";
        public const string VehicleMakeName = "vehicleMake";
        public const string VehicleModelName = "vehicleModel";
        public const string VehicleColor = "vehicleColor";
        public const string RequestType = "requestType";
        public const string TripNumber = "tripNumber";
        public const string TripStartDate = "tripDate";
        public const string ParticipantList = "participantList";
        public const string StatusUpdateLink = "statusUpdateLink";
        public const string RegistrationLink = "registrationLink";
        public const string RegistrationOpenDate = "registrationOpenDate";
        public const string PinCode = "pinCode";
        public const string MemberGroupName = "MemberGroupName";
    }
    public enum ArgRole { Unknown, Payload, Parameter }

}