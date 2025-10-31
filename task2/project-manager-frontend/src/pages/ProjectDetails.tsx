import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import api from "../api";

type Task = { id: number; title: string; isCompleted: boolean };
type Project = { id: number; title: string; description: string; tasks: Task[] };

export default function ProjectDetails() {
  const { id } = useParams();
  const [project, setProject] = useState<Project | null>(null);
  const [title, setTitle] = useState("");
  const navigate = useNavigate();

  const load = async () => {
    const res = await api.get(`/projects/${id}`);
    setProject(res.data);
  };

  useEffect(() => {
    load();
  }, [id]);

  const addTask = async () => {
    if (!title.trim()) return;
    await api.post(`/tasks/${id}`, { title });
    setTitle(""); load();
  };

  const toggleTask = async (taskId: number) => {
    await api.put(`/tasks/${taskId}/toggle`);
    load();
  };

  const deleteTask = async (taskId: number) => {
    await api.delete(`/tasks/${taskId}`);
    load();
  };

  return (
    <div className="p-4">
      <button onClick={() => navigate("/")}>← Back</button>
      {project && (
        <>
          <h1 className="text-2xl mb-2">{project.title}</h1>
          <p className="mb-4">{project.description}</p>

          <div className="mb-4">
            <input
              placeholder="New task title"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
            />
            <button onClick={addTask}>Add Task</button>
          </div>

          <ul>
            {project.tasks.map((t) => (
              <li key={t.id}>
                <input
                  type="checkbox"
                  checked={t.isCompleted}
                  onChange={() => toggleTask(t.id)}
                />
                <span
                  style={{
                    textDecoration: t.isCompleted ? "line-through" : undefined,
                    marginLeft: "6px",
                  }}
                >
                  {t.title}
                </span>
                <button
                  onClick={() => deleteTask(t.id)}
                  style={{ marginLeft: "10px" }}
                >
                  Delete
                </button>
              </li>
            ))}
          </ul>
        </>
      )}
    </div>
  );
}
