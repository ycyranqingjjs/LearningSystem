﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeiSha.Common;
using Song.ServiceInterfaces;
using VTemplate.Engine;
namespace Song.Site
{
    /// <summary>
    /// 课程购买
    /// </summary>
    public class CourseBuy : BasePage
    {
        //课程ID
        private int couid = WeiSha.Common.Request.QueryString["couid"].Int32 ?? 0;
        protected override void InitPageTemplate(HttpContext context)
        {
            //判断，如果已经购买，则直接跳转
            if (Extend.LoginState.Accounts.IsLogin)
            {
                bool isBuy = Business.Do<ICourse>().IsBuy(couid, Extend.LoginState.Accounts.CurrentUser.Ac_ID, 1);
                if (isBuy)
                {                                    
                    this.Response.Redirect("/CourseStudy.ashx?couid=" + couid);
                    return;
                }
            }
            //当前课程
            Song.Entities.Course course = Business.Do<ICourse>().CourseSingle(couid);
            if (course != null)
            {
                course.Cou_Logo = Upload.Get["Course"].Virtual + course.Cou_Logo;
                course.Cou_LogoSmall = Upload.Get["Course"].Virtual + course.Cou_LogoSmall;
            }
            this.Document.Variables.SetValue("course", course);
            if (course == null) return;
            //所属专业
            Song.Entities.Subject subject = Business.Do<ISubject>().SubjectSingle(course.Sbj_ID);
            this.Document.Variables.SetValue("subject", subject);
            //章节数
            int olCount = Business.Do<IOutline>().OutlineOfCount(course.Cou_ID, 0, true);
            this.Document.Variables.SetValue("olCount", olCount);
            //试题数
            int quesCount = Business.Do<IQuestions>().QuesOfCount(this.Organ.Org_ID, course.Sbj_ID, course.Cou_ID, 0, 0, true);
            this.Document.Variables.SetValue("quesCount", quesCount);
            //价格
            Song.Entities.CoursePrice[] prices = Business.Do<ICourse>().PriceCount(-1, course.Cou_UID, true, 0);
            this.Document.Variables.SetValue("prices", prices);
            //上级专业
            List<Song.Entities.Subject> sbjs = Business.Do<ISubject>().Parents(course.Sbj_ID, true);
            this.Document.Variables.SetValue("sbjs", sbjs);
        }
              
    }
}