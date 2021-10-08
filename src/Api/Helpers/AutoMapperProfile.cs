using AutoMapper;
using Domain.Entities;
using Domain.Models.Accounts;
using Domain.Models.Courses;

namespace Api.Helpers
{
    public class AutoMapperProfile : Profile
    {
        // mappings between model and entity objects
        public AutoMapperProfile()
        {
            AccountProfiles();
            CoursesProfile();
        }

        private void AccountProfiles()
        {
            CreateMap<Account, AccountResponse>();

            CreateMap<Account, AuthenticateResponse>();

            CreateMap<RegisterRequest, Account>();

            CreateMap<CreateRequest, Account>();

            CreateMap<UpdateRequest, Account>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                        // ignore null role
                        if (x.DestinationMember.Name == "Role" && src.Role == null) return false;

                        return true;
                    }
                ));
        }

        private void CoursesProfile()
        {
            CreateMap<Chapter, ChapterResponse>();
            CreateMap<Course, CourseResponse>();

            CreateMap<CreateChapter, Chapter>();
            CreateMap<CreateCourseRequest, Course>();

            CreateMap<UpdateChapter, Chapter>();
            CreateMap<UpdateCourseRequest, Course>();
        }
    }
}
