using CourseProvider.Infrastructure.Data.Contexts;
using CourseProvider.Infrastructure.Factories;
using CourseProvider.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseProvider.Infrastructure.Services;

//ett kontrakt på CRUD - det jag behöver skapa för att få det till att fungera
public interface ICourseService
{
    Task<Course> CreateCourseAsync(CourseCreateRequest request);
    Task<Course> GetCourseByIdAsync(string id);
    Task<IEnumerable<Course>> GetCoursesAsync();
    Task<Course> UpdateCourseAsync(CourseUpdateRequest request);
    Task<bool> DeleteCourseAsync(string id);
}


public class CourseService(IDbContextFactory<DataContext> contextFactory) : ICourseService
{
    private readonly IDbContextFactory<DataContext> _contextFactory = contextFactory;



    public async Task<Course> CreateCourseAsync(CourseCreateRequest request)
    {
        //Get access to db
        await using var context = _contextFactory.CreateDbContext();

        //Omvandla/populera. variabel courseEtntiyt, skicka request till CourseFactory create
        var courseEntity = CourseFactory.Create(request);

        //lägga till i db
        context.Courses.Add(courseEntity);

        //spara ändringar i db
        await context.SaveChangesAsync();

        //returnera den nya entiteten
        return CourseFactory.Create(courseEntity);
    }


    public async Task<Course> GetCourseByIdAsync(string id)
    {
        //get access to DB
        await using var context = _contextFactory.CreateDbContext();
        //search
        var courseEntity = await context.Courses.FirstOrDefaultAsync(x => x.Id == id);

        //return entity if true
        return courseEntity == null ? null! : CourseFactory.Create(courseEntity);

    }

    public async Task<IEnumerable<Course>> GetCoursesAsync()
    {
        //get access to DB
        await using var context = _contextFactory.CreateDbContext();

        //Get courses from courses in context, to a list
        var courseEntities = await context.Courses.ToListAsync();

        //return entities with select through courseFactory
        return courseEntities.Select(CourseFactory.Create);
    }

    public async Task<Course> UpdateCourseAsync(CourseUpdateRequest request)
    {
        //get access to DB
        await using var context = _contextFactory.CreateDbContext();

        //search for existing course
        var existingCourse = await context.Courses.FirstOrDefaultAsync(x => x.Id == request.Id);

        //return null if null
        if (existingCourse == null) return null!;



        //create the UpdatedEntity through CourseFactory, create with request
        var updatedCourseEntity = CourseFactory.Create(request);

        //update the existing entity with the updated
        updatedCourseEntity.Id = existingCourse.Id;

        //set the new values to the entity
        context.Entry(existingCourse).CurrentValues.SetValues(updatedCourseEntity);


        //save changes
        await context.SaveChangesAsync();

        //return The updated entity
        return CourseFactory.Create(existingCourse);

    }
    public async Task<bool> DeleteCourseAsync(string id)
    {
        //get access to db
        await using var context = _contextFactory.CreateDbContext();

        //sök efter courseEntitys Id
        var courseEntity = await context.Courses.FirstOrDefaultAsync(x => x.Id == id);

        //returnera false om entiteten är null
        if (courseEntity == null) return false;

        //ta bort id ur context/db
        context.Courses.Remove(courseEntity);

        //spara ändringar
        await context.SaveChangesAsync();

        //returnera true
        return true;

    }
}
