import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../api";

type Project = { id: number; title: string; description: string };

export default function Dashboard() {
  const [projects, setProjects] = useState<Project[]>([]);
  const [title, setTitle] = useState("");
  const [desc, setDesc] = useState("");
  const navigate = useNavigate();

  const load = async () => {
    const res = await api.get("/projects");
    setProjects(res.data);
  };

  useEffect(() => {
    load();
  }, []);

  const addProject = async () => {
    if (!title.trim()) return;
    await api.post("/projects", { title, description: desc });
    setTitle(""); setDesc(""); load();
  };

  const logout = () => {
    localStorage.removeItem("token");
    navigate("/login");
  };

  return (
    <div className="p-4">
      <div className="flex justify-between items-center mb-4">
        <h1 className="text-2xl">My Projects</h1>
        <button onClick={logout}>Logout</button>
      </div>

      <div className="mb-4">
        <input
          placeholder="Project title"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
        />
        <input
          placeholder="Description"
          value={desc}
          onChange={(e) => setDesc(e.target.value)}
        />
        <button onClick={addProject}>Add</button>
      </div>

      <ul>
        {projects.map((p) => (
          <li key={p.id}>
            <span
              style={{ cursor: "pointer", color: "blue" }}
              onClick={() => navigate(`/projects/${p.id}`)}
            >
              {p.title}
            </span>
          </li>
        ))}
      </ul>
    </div>
  );
}
