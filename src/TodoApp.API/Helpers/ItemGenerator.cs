using TodoApp.Api.Data;

namespace TodoApp.Api.Helpers;

public class ItemGenerator
{
    private static readonly string[] Titles =
    [
        "Learn Python", "Explore JavaScript", "Master Java", "Dive into C++", "Understand React",
        "Discover Angular", "Conquer Vue.js", "Get to know Node.js", "Study Swift", "Investigate Kotlin",
        "Uncover Scala", "Peruse Perl", "Inspect Ruby", "Examine PHP", "Scrutinize TypeScript",
        "Grasp HTML5", "Seize CSS3", "Fathom SQL", "Probe NoSQL", "Master MongoDB",
        "Understand Docker", "Learn Kubernetes", "Explore Microservices", "Study GraphQL", "Investigate Redux",
        "Discover WebAssembly", "Conquer Electron", "Get to know TensorFlow", "Study PyTorch", "Investigate Flutter",
        "Uncover Xamarin", "Peruse AWS", "Inspect Azure", "Examine Google Cloud", "Scrutinize Firebase",
        "Grasp Git", "Seize Webpack", "Fathom Babel", "Probe Jenkins", "Master CircleCI",
        "Understand Selenium", "Learn Puppeteer", "Explore Jest", "Study Mocha", "Investigate Chai",
        "Discover Sinon", "Conquer Cypress", "Get to know Postman", "Study Swagger", "Investigate OAuth",
        "Uncover JWT", "Peruse SAML", "Inspect OpenID Connect", "Examine Apache Kafka", "Scrutinize RabbitMQ",
        "Grasp NGINX", "Seize Express.js", "Fathom FastAPI", "Probe Spring Boot", "Master Flask",
        "Understand Gatsby", "Learn Next.js", "Explore VuePress", "Study Nuxt.js", "Investigate Jekyll",
        "Discover Hugo", "Conquer Bootstrap", "Get to know Materialize", "Study Tailwind CSS", "Investigate Foundation",
        "Uncover Bulma", "Peruse Svelte", "Inspect Elm", "Examine PWA", "Scrutinize AMP",
        "Grasp WebGL", "Seize Three.js", "Fathom D3.js", "Probe Unity", "Master Unreal Engine",
        "Understand Blender", "Learn Sketch", "Explore Figma", "Study Adobe XD", "Investigate Photoshop",
        "Discover Illustrator", "Conquer InDesign", "Get to know Lightroom", "Study Premiere Pro",
        "Investigate After Effects"
    ];

    private static readonly Random Random = new();

    public static IEnumerable<Item> GenerateItems() => GenerateItems(Titles.Length);
    
    public static IEnumerable<Item> GenerateItems(int size)
    {
        if (size > Titles.Length)
        {
           throw new ArgumentOutOfRangeException(nameof(size),
               "Size must be less than or equal to the number of titles");
        }

        var usedTitles = new HashSet<string>();

        return Enumerable.Range(0, size).Select(_ =>
        {
           var title = Titles[Random.Next(0, Titles.Length)];
           while (!usedTitles.Add(title))
           {
               title = Titles[Random.Next(0, Titles.Length)];
           }

           return new Item
           {
               Title = title,
               Progress = Random.Next(0, 101),
               Priority = Random.Next(1, 6),
               DueDate = DateTime.Now.AddHours(Random.Next(-73, 73))
           };
        }).ToArray();
   }
}