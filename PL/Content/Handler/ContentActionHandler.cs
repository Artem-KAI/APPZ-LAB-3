using BLL.DTO;
using BLL.Services;
using PL.Menu;

namespace PL.Content.Handler
{
    public static class ContentActionHandler
    {
        public static bool Handle(string input, BlogService service, UserDTO? user, ArticleDTO article, Dictionary<string, int> indexMap)
        {
            try
            {
                switch (input)
                {
                    case "1":
                        if (user == null) return true;
                        Console.Write("Ваш коментар: ");
                        service.CreateComment(user.Id, article.Id, Console.ReadLine());
                        return true;

                    case "R":
                        if (user == null) return true;
                        HandleReply(service, user, article, indexMap);
                        return true;

                    case "X":
                        if (user == null) return true;
                        HandleDeleteComment(service, user, article, indexMap);
                        return true;

                    case "D":
                        if (user != null && article.AuthorName == user.Nickname)
                        {
                            Console.Write("Видалити статтю? (y/n): ");
                            if (Console.ReadLine()?.ToLower() == "y")
                            {
                                service.DeleteArticle(user.Id, article.Id);
                                return false;
                            }
                        }
                        return true;

                    case "0":
                        return false;
                }
            }
            catch (Exception ex)
            {
                MenuHelper.ShowError(ex.Message);
            }
            return true;
        }

        private static void HandleReply(BlogService service, UserDTO user, ArticleDTO article, Dictionary<string, int> indexMap)
        {
            Console.Write("Введіть номер коментаря (напр. 1 або 2.1): ");
            string idx = Console.ReadLine();
            if (indexMap.ContainsKey(idx))
            {
                Console.Write($"Ваша відповідь на [{idx}]: ");
                service.ReplyToComment(user.Id, article.Id, indexMap[idx], Console.ReadLine());
                MenuHelper.ShowSuccess("Відповідь додано!");
            }
        }

        private static void HandleDeleteComment(BlogService service, UserDTO user, ArticleDTO article, Dictionary<string, int> indexMap)
        {
            Console.Write("Введіть номер вашого коментаря для видалення: ");
            string idx = Console.ReadLine();
            if (indexMap.ContainsKey(idx))
            {
                service.DeleteComment(user.Id, indexMap[idx]);
                MenuHelper.ShowSuccess("Видалено!");
            }
        }
    }
}
