using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class DbInitializer
    {
        private record SeedUser(string DisplayName, string Email, string Bio, string ImageUrl);

        private record ActivityRename(string OldTitle, string Title, string Description);

        public static async Task SeedData(AppDBContext context, UserManager<User> userManager)
        {
            var seedUsers = new List<SeedUser>
            {
                new("Bob", "bob@test.com", "Pub quiz champion and live music lover. Always up for a pint after work.", Portrait("men", 22)),
                new("Tom", "tom@test.com", "Museum hopper, amateur photographer and weekend cyclist.", Portrait("men", 36)),
                new("Jane", "jane@test.com", "Travel addict with 30 countries and counting. I organise trips nobody forgets.", Portrait("women", 68)),
                new("Sara", "sara@test.com", "Foodie, home baker and street-food hunter. Show me the best shawarma in town!", Portrait("women", 44)),
                new("Omar", "omar@test.com", "Desert explorer and photographer. Sunsets are my favourite kind of meeting.", Portrait("men", 32)),
                new("Lina", "lina@test.com", "Architect by day, stargazer by night. I love art, design and long road trips.", Portrait("women", 65)),
                new("Ahmed", "ahmed@test.com", "Software engineer who cooks a mean kabsa. Big fan of football and board games.", Portrait("men", 75)),
                new("Emma", "emma@test.com", "Jazz singer and vinyl collector. Find me in the front row at every gig.", Portrait("women", 12)),
                new("Lucas", "lucas@test.com", "Film buff and festival regular. I will happily talk about Kubrick for hours.", Portrait("men", 46)),
                new("Mia", "mia@test.com", "Pastry chef exploring every market in the city. Coffee first, always.", Portrait("women", 90)),
                new("Yusuf", "yusuf@test.com", "Specialty coffee roaster and weekend hiker. Let's go for a brew.", Portrait("men", 11)),
            };

            var users = new Dictionary<string, User>();

            foreach (var seed in seedUsers)
            {
                var user = await userManager.FindByEmailAsync(seed.Email);
                if (user == null)
                {
                    user = new User
                    {
                        DisplayName = seed.DisplayName,
                        UserName = seed.Email,
                        Email = seed.Email,
                        Bio = seed.Bio,
                        ImageUrl = seed.ImageUrl
                    };
                    await userManager.CreateAsync(user, "Pa$$w0rd");
                }
                else
                {
                    if (string.IsNullOrEmpty(user.Bio) || user.Bio.StartsWith("Bio of"))
                        user.Bio = seed.Bio;
                    user.ImageUrl ??= seed.ImageUrl;
                }

                if (user.ImageUrl == seed.ImageUrl && !await context.Photos.AnyAsync(x => x.Url == seed.ImageUrl))
                {
                    context.Photos.Add(new Photo
                    {
                        Url = seed.ImageUrl,
                        PublicId = $"seed_{seed.DisplayName.ToLower()}",
                        UserId = user.Id
                    });
                }

                users[seed.DisplayName] = user;
            }

            await context.SaveChangesAsync();

            var renames = new List<ActivityRename>
            {
                new("Past Activity 1", "Friday Pints at the Lamb & Flag", "Kick off the weekend in one of Covent Garden's oldest pubs. Great ales, better company."),
                new("Past Activity 2", "Louvre Late-Night Tour", "A guided evening walk through the Louvre's masterpieces, from the Mona Lisa to the Winged Victory."),
                new("Future Activity 1", "Dinosaurs After Dark", "Explore the Natural History Museum after hours with talks, drinks and the famous blue whale."),
                new("Future Activity 2", "Arena Night at The O2", "Big lights, bigger sound. Join us for a headline show at The O2 arena."),
                new("Future Activity 3", "Riverside Drinks at The Mayflower", "Sundowners on the deck of London's historic riverside pub where the Pilgrims set sail."),
                new("Future Activity 4", "Craft Beer Tasting at The Blackfriar", "Sample six local craft beers inside one of London's most beautiful Art Nouveau pubs."),
                new("Future Activity 5", "Sherlock Holmes Mystery Walk", "Solve clues across Marylebone before ending at 221b Baker Street. Deerstalkers optional."),
                new("Future Activity 6", "Indie Gig at the Roundhouse", "Discover the next big indie bands in Camden's legendary circular venue."),
                new("Future Activity 7", "Thames Kayak Adventure", "Paddle along the quiet upper Thames with an expert guide. All levels welcome."),
                new("Future Activity 8", "Premiere Night at Odeon Leicester Square", "Red carpet vibes for the latest blockbuster premiere, followed by a film chat over dinner."),
            };

            foreach (var rename in renames)
            {
                var existing = await context.Activities.FirstOrDefaultAsync(x => x.Title == rename.OldTitle);
                if (existing == null) continue;
                existing.Title = rename.Title;
                existing.Description = rename.Description;
                if (existing.Venue == "Odeon Leicester Square")
                {
                    existing.Latitude = 51.5103;
                    existing.Longitude = -0.1301;
                }
            }

            await context.SaveChangesAsync();

            var activities = BuildActivities(users);
            var existingTitles = await context.Activities.Select(x => x.Title).ToListAsync();
            var newActivities = activities.Where(x => !existingTitles.Contains(x.Title)).ToList();

            context.Activities.AddRange(newActivities);

            foreach (var activity in newActivities)
            {
                foreach (var (author, body, minutesAgo) in BuildComments(activity.Title))
                {
                    context.Comments.Add(new Comment
                    {
                        Body = body,
                        UserId = users[author].Id,
                        ActivityId = activity.Id,
                        CreatedAt = DateTime.UtcNow.AddMinutes(-minutesAgo)
                    });
                }
            }

            var followings = new (string Observer, string Target)[]
            {
                ("Bob", "Tom"), ("Bob", "Jane"), ("Bob", "Emma"), ("Tom", "Bob"), ("Tom", "Mia"),
                ("Jane", "Bob"), ("Jane", "Sara"), ("Jane", "Lina"), ("Sara", "Omar"), ("Sara", "Mia"),
                ("Sara", "Jane"), ("Omar", "Lina"), ("Omar", "Yusuf"), ("Omar", "Sara"), ("Lina", "Omar"),
                ("Lina", "Jane"), ("Ahmed", "Omar"), ("Ahmed", "Sara"), ("Ahmed", "Yusuf"), ("Emma", "Lucas"),
                ("Emma", "Bob"), ("Lucas", "Emma"), ("Lucas", "Mia"), ("Mia", "Sara"), ("Mia", "Lucas"),
                ("Yusuf", "Sara"), ("Yusuf", "Ahmed"), ("Yusuf", "Omar")
            };

            var existingFollowings = await context.UserFollowings
                .Select(x => x.ObserverId + "|" + x.TargetId)
                .ToListAsync();

            foreach (var (observer, target) in followings)
            {
                if (existingFollowings.Contains(users[observer].Id + "|" + users[target].Id)) continue;
                context.UserFollowings.Add(new UserFollowing
                {
                    ObserverId = users[observer].Id,
                    TargetId = users[target].Id
                });
            }

            await context.SaveChangesAsync();
        }

        private static string Portrait(string gender, int number) =>
            $"https://randomuser.me/api/portraits/{gender}/{number}.jpg";

        private static List<ActivityAttendee> Attendees(Dictionary<string, User> users, string host, params string[] others)
        {
            var attendees = new List<ActivityAttendee> { new() { UserId = users[host].Id, IsHost = true } };
            attendees.AddRange(others.Select(x => new ActivityAttendee { UserId = users[x].Id }));
            return attendees;
        }

        private static Activity NewActivity(string title, DateTime date, string description, string category,
            string city, string venue, double latitude, double longitude, List<ActivityAttendee> attendees) => new()
        {
            Title = title,
            Date = date,
            Description = description,
            Category = category,
            City = city,
            Venue = venue,
            Latitude = latitude,
            Longitude = longitude,
            Attendees = attendees
        };

        private static List<Activity> BuildActivities(Dictionary<string, User> users)
        {
            var now = DateTime.UtcNow.Date.AddHours(18);

            return
            [
                NewActivity("Friday Pints at the Lamb & Flag", now.AddMonths(-2),
                    "Kick off the weekend in one of Covent Garden's oldest pubs. Great ales, better company.",
                    "drinks", "London", "The Lamb and Flag, 33 Rose Street, Covent Garden, London",
                    51.51171665, -0.1256611057818921, Attendees(users, "Bob", "Tom")),
                NewActivity("Louvre Late-Night Tour", now.AddMonths(-1),
                    "A guided evening walk through the Louvre's masterpieces, from the Mona Lisa to the Winged Victory.",
                    "culture", "Paris", "Louvre Museum, Rue Saint-Honoré, Paris",
                    48.8611473, 2.33802768704666, Attendees(users, "Tom", "Jane", "Bob")),
                NewActivity("Dinosaurs After Dark", now.AddMonths(1),
                    "Explore the Natural History Museum after hours with talks, drinks and the famous blue whale.",
                    "culture", "London", "Natural History Museum",
                    51.496510900000004, -0.17600190725447445, Attendees(users, "Jane")),
                NewActivity("Arena Night at The O2", now.AddMonths(2),
                    "Big lights, bigger sound. Join us for a headline show at The O2 arena.",
                    "music", "London", "The O2",
                    51.502936649999995, 0.0032029278126681844, Attendees(users, "Bob", "Jane")),
                NewActivity("Riverside Drinks at The Mayflower", now.AddMonths(3),
                    "Sundowners on the deck of London's historic riverside pub where the Pilgrims set sail.",
                    "drinks", "London", "The Mayflower",
                    51.501778, -0.053577, Attendees(users, "Tom")),
                NewActivity("Craft Beer Tasting at The Blackfriar", now.AddMonths(4),
                    "Sample six local craft beers inside one of London's most beautiful Art Nouveau pubs.",
                    "drinks", "London", "The Blackfriar",
                    51.512146650000005, -0.10364680647106028, Attendees(users, "Jane", "Bob")),
                NewActivity("Sherlock Holmes Mystery Walk", now.AddMonths(5),
                    "Solve clues across Marylebone before ending at 221b Baker Street. Deerstalkers optional.",
                    "culture", "London", "Sherlock Holmes Museum, 221b Baker Street, London",
                    51.5237629, -0.1584743, Attendees(users, "Bob")),
                NewActivity("Indie Gig at the Roundhouse", now.AddMonths(6),
                    "Discover the next big indie bands in Camden's legendary circular venue.",
                    "music", "London", "Roundhouse, Chalk Farm Road, Camden, London",
                    51.5432505, -0.15197608174931165, Attendees(users, "Tom", "Bob")),
                NewActivity("Thames Kayak Adventure", now.AddMonths(7),
                    "Paddle along the quiet upper Thames with an expert guide. All levels welcome.",
                    "travel", "London", "River Thames, England, United Kingdom",
                    51.5575525, -0.781404, Attendees(users, "Jane", "Tom")),
                NewActivity("Premiere Night at Odeon Leicester Square", now.AddMonths(8),
                    "Red carpet vibes for the latest blockbuster premiere, followed by a film chat over dinner.",
                    "film", "London", "Odeon Leicester Square",
                    51.5103, -0.1301, Attendees(users, "Bob")),
                NewActivity("Coffee Cupping Workshop", now.AddDays(3),
                    "Taste single-origin beans side by side and learn how roasters score coffee like the pros.",
                    "drinks", "Riyadh", "Al Olaya, Riyadh, Saudi Arabia",
                    24.6995, 46.6850, Attendees(users, "Yusuf", "Sara", "Mia", "Ahmed")),
                NewActivity("Street Food Crawl in Al-Balad", now.AddDays(5),
                    "Wander the coral-stone alleys of historic Jeddah tasting mutabbaq, foul and fresh sobia.",
                    "food", "Jeddah", "Al-Balad Historic District, Jeddah, Saudi Arabia",
                    21.4858, 39.1925, Attendees(users, "Sara", "Ahmed", "Lina", "Bob")),
                NewActivity("Borough Market Tasting Tour", now.AddDays(7),
                    "Cheese, oysters, fresh bread and the best toasties in London. Bring an empty stomach.",
                    "food", "London", "Borough Market, 8 Southwark Street, London",
                    51.5055, -0.0910, Attendees(users, "Mia", "Jane", "Lucas", "Emma", "Tom")),
                NewActivity("Jazz Night at Ronnie Scott's", now.AddDays(9),
                    "An evening of live jazz in Soho's most iconic club. Smart casual, good vibes.",
                    "music", "London", "Ronnie Scott's, 47 Frith Street, Soho, London",
                    51.5133, -0.1316, Attendees(users, "Emma", "Lucas", "Tom", "Jane")),
                NewActivity("Sunset Desert Safari", now.AddDays(12),
                    "Dune bashing, camel rides and a barbecue dinner under the stars in the Arabian desert.",
                    "travel", "Dubai", "Dubai Desert Conservation Reserve, Dubai, UAE",
                    24.8240, 55.7260, Attendees(users, "Omar", "Sara", "Yusuf", "Lina")),
                NewActivity("Open-Air Cinema: Classics Under the Stars", now.AddDays(15),
                    "Blankets, popcorn and a cult classic on a rooftop with the London skyline behind the screen.",
                    "film", "London", "Rooftop Film Club, Peckham, London",
                    51.4710, -0.0690, Attendees(users, "Lucas", "Emma", "Mia", "Bob")),
                NewActivity("Arabic Calligraphy Workshop", now.AddDays(18),
                    "Learn the basics of Thuluth and Diwani scripts with a master calligrapher. All materials provided.",
                    "culture", "Jeddah", "Hayy Jameel, Jeddah, Saudi Arabia",
                    21.5950, 39.1550, Attendees(users, "Omar", "Lina", "Sara")),
                NewActivity("Riyadh Boulevard Evening", now.AddDays(20),
                    "Lights, live shows and food trucks. A night out at the heart of Riyadh Season.",
                    "culture", "Riyadh", "Boulevard City, Riyadh, Saudi Arabia",
                    24.7680, 46.6025, Attendees(users, "Ahmed", "Omar", "Yusuf", "Lina")),
                NewActivity("Modern Art at Tate Modern", now.AddDays(25),
                    "A relaxed walk through the latest exhibitions, finishing with coffee on the river terrace.",
                    "culture", "London", "Tate Modern, Bankside, London",
                    51.5076, -0.0994, Attendees(users, "Tom", "Mia", "Lucas")),
                NewActivity("AlUla Stargazing Trip", now.AddDays(35),
                    "A weekend among ancient sandstone tombs, ending with a telescope session in one of the darkest skies on earth.",
                    "travel", "AlUla", "Gharameel, AlUla, Saudi Arabia",
                    26.7020, 37.9780, Attendees(users, "Lina", "Omar", "Ahmed", "Sara", "Yusuf")),
                NewActivity("Acoustic Session at Jazz Café", now.AddDays(40),
                    "Stripped-back sets from up-and-coming songwriters in Camden's favourite music venue.",
                    "music", "London", "Jazz Café, 5 Parkway, Camden, London",
                    51.5392, -0.1440, Attendees(users, "Bob", "Tom", "Emma")),
                NewActivity("Seine River Cruise", now.AddDays(45),
                    "Watch Paris light up from the water as the Eiffel Tower sparkles on the hour.",
                    "travel", "Paris", "Port de la Bourdonnais, Paris, France",
                    48.8606, 2.2930, Attendees(users, "Jane", "Emma", "Bob")),
                NewActivity("Seafood Dinner on the Corniche", now.AddDays(-10),
                    "Fresh catch of the day grilled by the Red Sea, with a stroll along the waterfront after.",
                    "food", "Jeddah", "Jeddah Corniche, Jeddah, Saudi Arabia",
                    21.5433, 39.1100, Attendees(users, "Ahmed", "Sara", "Omar")),
                NewActivity("Indie Film Festival Screening", now.AddDays(-20),
                    "Short films from emerging Arab directors followed by a Q&A with the filmmakers.",
                    "film", "Dubai", "Cinema Akil, Alserkal Avenue, Dubai, UAE",
                    25.1408, 55.2260, Attendees(users, "Mia", "Lucas", "Yusuf"))
            ];
        }

        private static List<(string Author, string Body, int MinutesAgo)> BuildComments(string title) => title switch
        {
            "Coffee Cupping Workshop" =>
            [
                ("Sara", "Can't wait! Will there be Ethiopian beans?", 300),
                ("Yusuf", "Yes, a natural Yirgacheffe and a washed Colombian.", 240),
                ("Mia", "Saving my taste buds for this one", 60)
            ],
            "Street Food Crawl in Al-Balad" =>
            [
                ("Ahmed", "I know a great mutabbaq stall near Bab Makkah.", 500),
                ("Lina", "Let's end with sobia please!", 120)
            ],
            "Borough Market Tasting Tour" =>
            [
                ("Jane", "First stop should be the cheese toasties.", 720),
                ("Lucas", "Agreed. Then oysters.", 650),
                ("Emma", "I'll bring the playlist for the walk", 90)
            ],
            "Sunset Desert Safari" =>
            [
                ("Yusuf", "Bringing my camera for the sunset shots", 1000),
                ("Omar", "Pickup is at 3pm sharp from the hotel lobby.", 400)
            ],
            "AlUla Stargazing Trip" =>
            [
                ("Ahmed", "Is it cold at night there?", 2000),
                ("Lina", "Very! Bring a jacket.", 1900)
            ],
            "Jazz Night at Ronnie Scott's" =>
            [
                ("Lucas", "Booked a table near the stage.", 300)
            ],
            _ => []
        };
    }
}
