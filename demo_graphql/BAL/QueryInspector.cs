namespace demo_graphql.BAL
{
    public class QueryInspector
    {

        public static class PostGresQuery
        {
            public const string Get_request_meta = @"SELECT
                                                            id,
                                                            object_name,
                                                            type,
                                                            category,
                                                            workflow_meta::text AS workflow_meta_raw,
                                                            custom_meta,
                                                            input_validation_meta::text AS inputValidation,
                                                            headers::text AS headers_raw,
                                                            permission_meta,
                                                            request_headers,
                                                            system_module_access_codes,
                                                            email_configuration::text AS email_configuration_raw
                                                        FROM public.request_meta
                                                        WHERE object_name = ANY(@queryList)
                                                        ORDER BY id;";
            public const string Get_dashboard_summary = @"WITH 
                                                employee_count AS (
                                                    SELECT COUNT(*) AS total_employees FROM employee
                                                ),
                                                department_count AS (
                                                    SELECT COUNT(*) AS total_departments FROM department
                                                ),
                                                salary_stats AS (
                                                    SELECT 
                                                        MIN(salary) AS min_salary,
                                                        MAX(salary) AS max_salary,
                                                        AVG(salary) AS avg_salary,
                                                        SUM(salary) AS total_salary
                                                    FROM employee
                                                ),
                                                address_summary AS (
                                                    SELECT 
                                                        COUNT(*) AS total_addresses,
                                                        COUNT(DISTINCT employee_id) AS employees_with_address
                                                    FROM employee_address
                                                )
                                                SELECT 
                                                    (SELECT total_employees FROM employee_count) AS TotalEmployees,
                                                    (SELECT total_departments FROM department_count) AS TotalDepartments,
                                                    (SELECT min_salary FROM salary_stats) AS MinSalary,
                                                    (SELECT max_salary FROM salary_stats) AS MaxSalary,
                                                    (SELECT avg_salary FROM salary_stats) AS AvgSalary,
                                                    (SELECT total_salary FROM salary_stats) AS TotalSalary,
                                                    (SELECT employees_with_address FROM address_summary) AS EmployeesWithAddress;
                                                    ";

            public const string ManageEmailQueue = @"INSERT INTO public.email_queue (
                                                        email_template_id,
                                                        title,
                                                        person_ids,
                                                        email_queue_status_id,
                                                        created_by,
                                                        created_at,
                                                        is_manual,
                                                        email_ids,
                                                        gender,
                                                        tag_ids,
                                                        schedule_start_date,
                                                        schedule_end_date,
                                                        occurence_type,
                                                        occurence_duration,
                                                        is_active,
                                                        system_generated
                                                    )
                                                    VALUES (
                                                        @email_template_id,
                                                        @title,
                                                        null,
                                                        @email_queue_status_id,
                                                        null,
                                                        NOW(),
                                                        false,
                                                        null,
                                                        null,
                                                        null,
                                                        null,
                                                        null,
                                                        null,
                                                        null,
                                                        TRUE,
                                                        false
                                                    )
                                                    RETURNING email_queue_id;
                                                ";

            public const string EmailQueueFrequencyUpdate = @"UPDATE public.email_queue
                                                    SET no_of_occurence =@NoOfOccurence
                                                    WHERE email_queue_id = @email_queue_id";


            public const string GetEmailQueueList = @"SELECT eq.email_queue_id AS EmailQueueId
                                                            ,eq.title AS EmailQueue
                                                            ,eq.email_ids AS EmailIds
                                                            ,eet.email_template_id AS EmailTemplateId
                                                            ,eet.title AS EmailTemplate 
                                                            ,eet.inbox_email_template_id AS Inbox_EmailTemplateId
                                                            ,eet.""previewBody"" AS PreviewBody
                                                            ,eet.body,eet.emailtypeid AS EmailTypeId
                                                            ,eet.targetapplication AS TargetApplication
                                                            ,eq.no_of_occurence AS NoOfOccurence
                                                            FROM public.email_queue eq
                                                            JOIN public.email_template eet  ON eet.email_template_id = eq.email_template_id
                                                            WHERE eq.email_queue_id = @EmailQueueId";

            public const string GetEmailType = @"SELECT id as EmailTypeId, et.title as Title, _dynamic FROM public.email_type et 
            WHERE (@EmailTypeId IS NULL OR et.id = @EmailTypeId)";

            public const string EmailQueueInsertWithCampaign = @"
                                                   INSERT INTO public.email_queue_member
                                                                                        (
                                                                                            email_queue_id,
                                                                                            member_id,
                                                                                            mis_id,
                                                                                            is_active,
                                                                                            created_at,
                                                                                            created_by,
                                                                                            updated_at,
                                                                                            updated_by,
                                                                                            campaign_run_id,
                                                                                            no_of_occurence
                                                                                        )
                                                                                        VALUES
                                                                                        (
                                                                                            @EmailQueueId,
                                                                                            @MemberId,
                                                                                            @MisId,
                                                                                            true,
                                                                                            null,
                                                                                            null,
                                                                                            null,
                                                                                            null,
                                                                                            @CampaignRunId,
                                                                                            @NoOfOccurence
                                                                                        );";


            public const string EmailQueueMemberUpdate = @"UPDATE email_queue_member
                                                           SET campaign_run_id = @CampaignRunId
                                                           WHERE email_queue_id = @EmailQueueId
                                                           AND member_id = ANY(@MemberIds::int[]);";
            public const string EmailQueueUpdate = @"UPDATE public.email_queue
                                                    SET campaign_run_id = 
                                                        CASE 
                                                            WHEN campaign_run_id IS NULL OR campaign_run_id = '' THEN @campaignRunId
                                                            ELSE campaign_run_id || ',' || @campaignRunId
                                                        END,
                                                        error_message = @errorMessage
                                                    WHERE email_queue_id = @EmailQueueId";

            public const string GetEmailQueue = @"SELECT email_queue_id, e.email_template_id,et.title AS EmailTemplate,  m.title AS MemberListType, e.title,et.emailtypeid,e.campaign_run_id as CampaignId,
                                              ,email_queue_status_id, e.created_by, e.created_at,
                                              ,CASE WHEN CHARINDEX('-',person_ids) > 0 THEN '' ELSE person_ids END person_ids,
                                              CASE WHEN CHARINDEX('-',person_ids) > 0 THEN SUBSTRING(person_ids,1,CHARINDEX('-',person_ids)-1)  ELSE '' END FromId, 
                                              CASE WHEN CHARINDEX('-',person_ids) > 0 THEN SUBSTRING(person_ids,CHARINDEX('-',person_ids)+1,20)  ELSE '' END ToId,
                                                 schedule_start_date,schedule_end_date,occurence_type,occurence_duration,e.is_active
                                              FROM public.email_queue e 
											   JOIN public.email_template et ON et.email_template_id = e.email_template_id
                                              WHERE e.is_manual=1 ORDER BY  e.email_queue_id DESC";


            public const string GetPersonOrEmail = @"
                                                        SELECT
                                                            642392 AS PersonId,
                                                            'alpesh.vaghasiya@baps.dev' AS Email,
                                                            'Sample Event' AS EventName,
                                                            'Alpesh' AS FirstName,
                                                            'Vaghasiya' AS LastName
                                                        FROM public.app_user p
                                                        where p.person_id = 138422
                                                        ";

        }
    }
}