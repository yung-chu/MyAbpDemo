using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MyAbpDemo.ApplicationDto;
using MyAbpDemo.Core;
using MyAbpDemo.Infrastructure;

namespace MyAbpDemo.Application
{
    public class PostAppService : AppServiceBase, IPostAppService
    {
        private readonly IBlogRepository _blogRepository;
        public PostAppService(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Result<GetPostsOutput>> GetPosts(int id)
        {
            var result = Result.FromData(new GetPostsOutput());

            if (id==0)
            {
                result.Code = ResultCode.Fail;
                result.Message = "请输入id";
                return result;
            }

            if (await _blogRepository.GetAll().AnyAsync(a => a.Id == id))
            {
                var data = new GetPostsOutput
                {
                    PostDtos = (await _blogRepository.GetAll().Include(a => a.Posts).FirstAsync(a => a.Id == id)).Posts.MapToList<PostDto>()
                };
                result.Data = data;
            }

            return result;
        }
    }
}
