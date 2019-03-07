using System.Collections.Generic;
using System.Text;

namespace LT.Component.Entity
{
    /// <summary>
    /// ExtHelper 通用类
    /// </summary>
    public class ExtHelper
    {
        /// <summary>
        /// 生成扩展搜索WHERE条件语句（以 AND 开头）
        /// </summary>
        /// <param name="querys"></param>
        /// <param name="sqlWhere"></param>
        public static void GenerateWhere(List<ExtModel.Query> querys, StringBuilder sqlWhere)
        {
            GenerateWhere(querys, 0, sqlWhere);
        }

        private static void GenerateWhere(List<ExtModel.Query> querys, int parentId, StringBuilder sqlWhere)
        {
            bool isHasChild = false;
            foreach (ExtModel.Query query in querys)
            {
                if (query.ParentId == parentId)
                {
                    if (query.ChildNodes > 0)
                    {
                        isHasChild = false;
                        foreach (ExtModel.Query q in querys)
                        {
                            if (q.ParentId == query.ListId)
                            {
                                isHasChild = true;
                                break;
                            }
                        }
                        if (isHasChild == true)
                        {
                            sqlWhere.Append(" ");
                            sqlWhere.Append(query.OP);
                            sqlWhere.Append(" (");
                            GenerateWhere(querys, query.ListId, sqlWhere);
                            sqlWhere.Append(")");
                        }
                    }
                    else
                    {
                        isHasChild = true;
                        if (query.ParentId > 0)
                        {
                            isHasChild = false;
                            foreach (ExtModel.Query q in querys)
                            {
                                if (q.ListId < query.ListId && q.ParentId == query.ParentId)
                                {
                                    isHasChild = true;
                                    break;
                                }
                            }
                        }
                        //若query.SQL将会出现 AND()的情况，因为SQL可能通过反射后为空 modify zyh 2009-5-20
                        if (query.SQL.Trim().Length > 0)
                        {
                            if (isHasChild == true)
                            {
                                sqlWhere.Append(" ");
                                sqlWhere.Append(query.OP);
                            }

                            sqlWhere.Append(" (");
                            sqlWhere.Append(query.SQL);
                            sqlWhere.Append(")");
                        }
                    }
                }
            }
        }
    }
}
