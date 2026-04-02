using BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL.Content.Render
{
    public static class CommentUI
    {
        public static Dictionary<string, int> RenderComments(List<CommentDTO> allComments)
        {
            var indexMap = new Dictionary<string, int>();
            Console.WriteLine("\nКОМЕНТАРІ:");

            if (!allComments.Any())
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("Коментарів ще немає.");
                Console.ResetColor();
                return indexMap;
            }

            var rootComments = allComments.Where(c => c.ParentId == null).ToList();
            for (int i = 0; i < rootComments.Count; i++)
            {
                var c = rootComments[i];
                string rootIdx = (i + 1).ToString();
                indexMap[rootIdx] = c.Id;

                Console.WriteLine($"> [{rootIdx}] {c.AuthorName}: {c.Text}");

                var replies = allComments.Where(r => r.ParentId == c.Id).ToList();
                for (int j = 0; j < replies.Count; j++)
                {
                    var r = replies[j];
                    string replyIdx = $"{rootIdx}.{j + 1}";
                    indexMap[replyIdx] = r.Id;
                    Console.WriteLine($"  └── [Reply to {c.AuthorName}] [{replyIdx}] {r.AuthorName}: {r.Text}");
                }
            }
            return indexMap;
        }

        public static void RenderNavigationHint(UserDTO? user, List<CommentDTO> comments)
        {
            Console.WriteLine("\n" + new string('-', 20));
            if (user == null)
            {
                Console.WriteLine("[0] Назад (Увійдіть, щоб коментувати)");
            }
            else
            {
                string hints = "[1] Коментувати | [R] Відповісти | [0] Назад";
                if (comments.Any(c => c.AuthorName == user.Nickname)) hints += " | [X] Видалити коментар";
                Console.WriteLine(hints);
            }
        }
    }
}
