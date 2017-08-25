﻿using System;
using AutoMapper;
using Umbraco.Core;
using Umbraco.Core.Models.Membership;
using Umbraco.Web.Models.ContentEditing;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Identity;
using Umbraco.Core.Security;
using Umbraco.Core.Services;

namespace Umbraco.Web.Models.Mapping
{
    internal class UserProfile : Profile
    {
        private readonly ILocalizedTextService _textService;

        public UserProfile(ILocalizedTextService textService)
        {
            _textService = textService;

            CreateMap<IUser, UserDetail>()
                .ForMember(detail => detail.UserId, opt => opt.MapFrom(user => GetIntId(user.Id)))
                .ForMember(detail => detail.UserType, opt => opt.MapFrom(user => user.UserType.Alias))
                .ForMember(detail => detail.StartContentId, opt => opt.MapFrom(user => user.StartContentId))
                .ForMember(detail => detail.StartMediaId, opt => opt.MapFrom(user => user.StartMediaId))
                .ForMember(detail => detail.Culture, opt => opt.MapFrom(user => user.GetUserCulture(_textService)))
                .ForMember(
                    detail => detail.EmailHash,
                    opt => opt.MapFrom(user => user.Email.ToLowerInvariant().Trim().ToMd5()))
                .ForMember(detail => detail.SecondsUntilTimeout, opt => opt.Ignore());

            CreateMap<BackOfficeIdentityUser, UserDetail>()
                .ForMember(detail => detail.UserId, opt => opt.MapFrom(user => user.Id))
                .ForMember(detail => detail.UserType, opt => opt.MapFrom(user => user.UserTypeAlias))
                .ForMember(detail => detail.StartContentId, opt => opt.MapFrom(user => user.StartContentId))
                .ForMember(detail => detail.StartMediaId, opt => opt.MapFrom(user => user.StartMediaId))
                .ForMember(detail => detail.Culture, opt => opt.MapFrom(user => user.Culture))
                .ForMember(detail => detail.AllowedSections, opt => opt.MapFrom(user => user.AllowedSections))
                .ForMember(
                    detail => detail.EmailHash,
                    opt => opt.MapFrom(user => user.Email.ToLowerInvariant().Trim().ToMd5()))
                .ForMember(detail => detail.SecondsUntilTimeout, opt => opt.Ignore());

            CreateMap<IProfile, UserBasic>()
                  .ForMember(detail => detail.UserId, opt => opt.MapFrom(profile => GetIntId(profile.Id)));

            CreateMap<IUser, UserData>()
                .ConstructUsing((IUser user) => new UserData())
                .ForMember(detail => detail.Id, opt => opt.MapFrom(user => user.Id))
                .ForMember(detail => detail.AllowedApplications, opt => opt.MapFrom(user => user.AllowedSections))
                .ForMember(detail => detail.RealName, opt => opt.MapFrom(user => user.Name))
                .ForMember(detail => detail.Roles, opt => opt.MapFrom(user => new[] {user.UserType.Alias}))
                .ForMember(detail => detail.StartContentNode, opt => opt.MapFrom(user => user.StartContentId))
                .ForMember(detail => detail.StartMediaNode, opt => opt.MapFrom(user => user.StartMediaId))
                .ForMember(detail => detail.Username, opt => opt.MapFrom(user => user.Username))
                .ForMember(detail => detail.Culture, opt => opt.MapFrom(user => user.GetUserCulture(_textService)))
                .ForMember(detail => detail.SessionId, opt => opt.MapFrom(user => user.SecurityStamp.IsNullOrWhiteSpace() ? Guid.NewGuid().ToString("N") : user.SecurityStamp));

        }

        private static int GetIntId(object id)
        {
            var result = id.TryConvertTo<int>();
            if (result.Success == false)
            {
                throw new InvalidOperationException(
                    "Cannot convert the profile to a " + typeof(UserDetail).Name + " object since the id is not an integer");
            }
            return result.Result;
        }

    }
}