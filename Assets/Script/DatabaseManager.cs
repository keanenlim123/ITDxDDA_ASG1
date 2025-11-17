using Firebase.Database;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    void Start()
    {
        // var justin = new Student();
        // justin.id = "S10270417C";
        // justin.name = "Keanen Lim Xi En";
        // justin.email = "S10270417C@connect.np.edu.sg";
        // justin.age = 20;
        // justin.phoneNumber = 93820569;
        // justin.gender = "Male";

        // var json = JsonUtility.ToJson(justin);

        // Debug.Log(json);
        // var db = FirebaseDatabase.DefaultInstance.RootReference;
        // db.Child("Student").SetRawJsonValueAsync(json);
    }

    //Create Student
    public void CreateStudent()
    {
        var justin = new Student();
        justin.id = "S10270417C";
        justin.name = "Keanen Lim Xi En";
        justin.email = "S10270417C@connect.np.edu.sg";
        justin.age = 15;
        justin.phoneNumber = 98765432;
        justin.gender = "Male";
        var db = FirebaseDatabase.DefaultInstance.RootReference;

        string json = JsonUtility.ToJson(justin);
        db.Child("Students").SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if (task.IsCompleted)
                Debug.Log("Student created successfully!");
        });
    }

    // Retrieve Student Data
    public void RetrieveStudent()
    {
        var db = FirebaseDatabase.DefaultInstance.RootReference;
        db.Child("Students").GetValueAsync().ContinueWith(task =>
        {

            string json = task.Result.GetRawJsonValue();
            Student student = JsonUtility.FromJson<Student>(json);

            Debug.Log(
                $"ID: {student.id}" +
                $" Name: {student.name}" +
                $" Email: {student.email}" +
                $" Age: {student.age}" +
                $" Phone: {student.phoneNumber}" +
                $" Gender: {student.gender}"
            );
        });
    }

    // Update Student Records
    public void UpdateStudent()
    {
        var db = FirebaseDatabase.DefaultInstance.RootReference;

        db.Child("Students").Child("email").SetValueAsync("keanenlimop@gmail.com");
        db.Child("Students").Child("age").SetValueAsync(67).ContinueWith(task =>
        {
            if (task.IsCompleted)
                Debug.Log("Student updated successfully!");
        });
        db.Child("Students").GetValueAsync().ContinueWith(task =>
        {

            string json = task.Result.GetRawJsonValue();
            Student student = JsonUtility.FromJson<Student>(json);

            Debug.Log(
                $"ID: {student.id}" +
                $" Name: {student.name}" +
                $" Email: {student.email}" +
                $" Age: {student.age}" +
                $" Phone: {student.phoneNumber}" +
                $" Gender: {student.gender}"
            );
        });
    }

    // Delete Student Records
    public void DeleteStudent()
    {
        var db = FirebaseDatabase.DefaultInstance.RootReference;

        db.Child("Students").RemoveValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
                Debug.Log("Student deleted successfully!");
        });
    }
}